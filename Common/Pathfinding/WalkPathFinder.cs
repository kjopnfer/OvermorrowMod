using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace OvermorrowMod.Common.Pathfinding
{
    public class WalkPathFinderInfo
    {
        public float StartYSpeed;
        public bool[] JumpSwitches;
        public bool IsJumpRight;
    }

    public enum AIState
    {
        Leaping,
        Idle,
        Falling,
        Moving
    }

    /// <summary>
    /// Test path finder only capable of walking on flat surfaces, moving up one and down one.
    /// </summary>
    public class WalkPathFinder : BasePathFinder<WalkPathFinderInfo>
    {
        private float[] _jumpSpeeds = new[] { 7f / 16f, 5.5f / 16f };
        private float _moveSpeed = 4f / 16f;
        private float _gravity = 0.3f / 16f;
        private int _maxFallDepth = 20;
        private float _maxFallSpeed = 10f / 16f;

        public WalkPathFinder(int xSize, int ySize, int timeout, int maxDivergence) : base(timeout, maxDivergence)
        {
            EntitySize = (xSize, ySize);
            BuildLeapTargets();
            _moveTargets = new[]
            {
                new RelativeCoordinate<WalkPathFinderInfo> { X = -1, Y = 0, Cost = 1f },
                new RelativeCoordinate<WalkPathFinderInfo> { X = 1, Y = 0, Cost = 1f },
                new RelativeCoordinate<WalkPathFinderInfo> { X = -1, Y = -1, Cost = 1f },
                new RelativeCoordinate<WalkPathFinderInfo> { X = 1, Y = -1, Cost = 1f }
            };
        }


        #region PathFinding
        private HashSet<RelativeCoordinate<WalkPathFinderInfo>> _leapTargets;
        private RelativeCoordinate<WalkPathFinderInfo>[] _moveTargets;

        // Number of jump-movement permutations, this number will blow up the potential movement complexity.
        // This should be a power of 2, i.e. 4 is 0b11 so we check permuations 00, 01, 10, 11.
        // 10 here indicates not moving in the first half, then moving in the second 
        private readonly int _numPermutations = 4;
        private readonly int _numPermutationsLog = 2;

        private void BuildLeapTargets()
        {
            _leapTargets = new HashSet<RelativeCoordinate<WalkPathFinderInfo>>();
            // x-axis movement direction
            foreach (float jumpSpeed in _jumpSpeeds)
            {
                for (int dir = -1; dir <= 1; dir += 2)
                {
                    for (int i = 0; i < _numPermutations; i++)
                    {
                        var permutations = GetPermutations(i, _numPermutations);
                        var numChanges = (int)Math.Log2(_numPermutations);
                        var arch = SimulateJumpArch(_gravity, -jumpSpeed, _maxFallSpeed, _moveSpeed * dir, numChanges, permutations);
                        var steps = arch.ToArray();
                        var lastStep = steps.Last();
                        _leapTargets.Add(new RelativeCoordinate<WalkPathFinderInfo>
                        {
                            Cost = lastStep.Cost,
                            Curve = steps,
                            X = (short)lastStep.XPos,
                            Y = (short)lastStep.YPos,
                            Info = new WalkPathFinderInfo
                            {
                                StartYSpeed = -jumpSpeed,
                                JumpSwitches = permutations.ToArray(),
                                IsJumpRight = dir > 0
                            }
                        });
                    }
                }
            }
        }

        private IEnumerable<bool> GetPermutations(int num, int maxPermutations)
        {
            for (int i = 0; i < (int)Math.Log2(maxPermutations); i++)
            {
                yield return (num & (1 << i)) != 0;
            }
        }

        private IEnumerable<JumpPathStep> SimulateJumpArch(float gravity, float initialY, float maxYSpeed, float xSpeed, int numChanges, IEnumerable<bool> permutation)
        {
            var arch = new HashSet<(int, int)>();

            float numSteps = Math.Abs(initialY * 2 / gravity);
            float stepsPerPermutation = numSteps / numChanges;
            float nextTreshold = stepsPerPermutation;


            float yv = initialY;
            var enumerator = permutation.GetEnumerator();
            enumerator.MoveNext();

            float xpos = 0, ypos = 0;
            int iter = 0;

            while (ypos < 0 && iter < 100 || iter == 0)
            {
                if (iter >= nextTreshold)
                {
                    nextTreshold += stepsPerPermutation;
                    enumerator.MoveNext();
                }
                float xv = enumerator.Current ? xSpeed : 0;
                int rxpos = (int)xpos;
                int rypos = (int)ypos;
                if (arch.Add((rxpos, rypos)))
                {
                    yield return new JumpPathStep
                    {
                        XPos = rxpos,
                        YPos = rypos,
                        IsFalling = yv >= 0,
                        Cost = iter * _moveSpeed * 1.1f
                    };
                }
                xpos += xv;
                ypos += yv;
                yv += gravity;

                iter++;
            }

        }

        protected override IEnumerable<RelativeCoordinate<WalkPathFinderInfo>> LeapTargets => _leapTargets;

        protected override bool IsBestEffort => true;

        protected override IEnumerable<RelativeCoordinate<WalkPathFinderInfo>> MoveTargets => _moveTargets;

        protected override (int X, int Y) EntitySize { get; }

        protected override IEnumerable<RelativeCoordinate<WalkPathFinderInfo>> GetFallTargets(int x, int y, WalkPathFinderInfo info)
        {
            float ySpeed = -(info?.StartYSpeed ?? 0);
            float maxXPos = x;
            float minXPos = x;

            float yPos = y;
            int lastYPos = (int)yPos;
            var passableColumns = new HashSet<int>();
            passableColumns.Add((int)maxXPos);

            int ticks = 0;
            while (yPos - y < _maxFallDepth && passableColumns.Any())
            {
                ticks++;
                ySpeed = Math.Min(_maxFallSpeed, ySpeed + _gravity);
                yPos += ySpeed;
                if ((int)yPos == lastYPos)
                {
                    continue;
                }
                lastYPos = (int)yPos;

                for (int i = (int)minXPos; i <= (int)maxXPos; i++)
                {
                    if (!passableColumns.Contains(i)) continue;
                    if (!CanFitInTile(base.State, i, (int)yPos))
                    {
                        passableColumns.Remove(i);
                        if (i == (int)minXPos) minXPos = (int)minXPos + 1;
                        if (i == (int)maxXPos) maxXPos = (int)maxXPos - 1;
                        continue;
                    }

                    if (CanStandOn(base.State, i, (int)yPos))
                    {
                        yield return new RelativeCoordinate<WalkPathFinderInfo>
                        {
                            Cost = ticks * _moveSpeed * 1.1f,
                            X = (short)(i - x),
                            Y = (short)((int)yPos - y)
                        };
                        if (!CanFallThrough(base.State, i, (int)yPos))
                        {
                            passableColumns.Remove(i);
                            if (i == (int)minXPos) minXPos = (int)minXPos + 1;
                            if (i == (int)maxXPos) maxXPos = (int)maxXPos - 1;
                            continue;
                        }
                    }
                }

                if (CanFitInTile(base.State, (int)minXPos - 1, (int)yPos))
                {
                    minXPos -= _moveSpeed;
                    passableColumns.Add((int)minXPos);
                }
                if (CanFitInTile(base.State, (int)maxXPos + 1, (int)yPos))
                {
                    maxXPos += _moveSpeed;
                    passableColumns.Add((int)maxXPos);
                }
            }
            
        }
        #endregion

        #region Execution
        public AIState State { get; private set; } = AIState.Idle;
        private int _tick;
        private int _numTicks;

        private int _lastX;
        private int _lastY;
        private int _targetX;
        private int _targetY;

        private PossibleMove<WalkPathFinderInfo> _activeMove;

        private AIState GetStateFromMove()
        {
            if (_activeMove == null) return AIState.Idle;
            if (_activeMove.IsLeap) return AIState.Leaping;
            if (_activeMove.IsFall) return AIState.Falling;
            return AIState.Moving;
        }

        private void TickNextMove(Vector2 position)
        {
            _activeMove = Next((int)(position.X / 16f),
                            (int)(position.Y / 16f),
                            _targetX,
                            _targetY);
            _tick = 0;
            State = GetStateFromMove();
        }

        public void SetTarget(Vector2 target)
        {
            _targetX = (int)(target.X / 16f);
            _targetY = ProjectToGround(_targetX, (int)(target.Y / 16f));
        }

        public Vector2 GetVelocity(Vector2 position, Vector2 velocity)
        {
            int roundX = (int)(position.X / 16f);
            int roundY = (int)(position.Y / 16f);
            bool isStill = State != AIState.Idle
                && (velocity.X != 0f || velocity.Y != 0f);
            // velocity.Y = Math.Min(_maxFallSpeed * 16f, velocity.Y + _gravity * 16f);
            if (isStill
                && roundX == _lastX
                && roundY == _lastY) return velocity;

            _lastX = roundX;
            _lastY = roundY;

            if (State != AIState.Idle && roundX == _targetX && roundY == _targetY)
            {
                _tick = 0;
                State = AIState.Idle;
                velocity.X = 0f;
                return velocity;
            }


            switch (State)
            {
                case AIState.Idle:
                    if (_tick++ > 120)
                    {
                        TickNextMove(position);
                        return GetVelocity(position, velocity);
                    }
                    return new Vector2(0, 0);
                case AIState.Leaping:
                    if (_tick == 0)
                    {
                        velocity.Y = _activeMove.Info.StartYSpeed * 16f - 0.5f;
                        _numTicks = (int)Math.Abs(_activeMove.Info.StartYSpeed * 2 / _gravity);
                    }
                    else if (velocity.Y == 0f && CanStandOn(base.State, roundX, roundY))
                    {
                        TickNextMove(position);
                        return GetVelocity(position, velocity);
                    }
                    int idx = (int)(_numPermutationsLog * (float)_tick / _numTicks);
                    
                    bool shouldMove = idx >= _numPermutationsLog ? false : _activeMove.Info.JumpSwitches[idx];
                    if (shouldMove)
                    {
                        velocity.X = _activeMove.Info.IsJumpRight ? _moveSpeed * 16f : -_moveSpeed * 16f;
                    }
                    _tick++;
                    return velocity;
                case AIState.Moving:
                    {
                        TickNextMove(position);
                        if (State == AIState.Moving)
                        {
                            float diff = _activeMove.XTarget * 16f - position.X + 8f;
                            if (diff > 0)
                            {
                                velocity.X = _moveSpeed * 16f;
                            }
                            else
                            {
                                velocity.X = -_moveSpeed * 16f;
                            }

                            float yDiff = _activeMove.YTarget * 16f - position.Y;
                            if (yDiff < -4f)
                            {
                                velocity.Y = -_moveSpeed * 16f;
                            }
                            else
                            {
                                velocity.Y = 0f;
                            }
                        }
                        else
                        {
                            return GetVelocity(position, velocity);
                        }
                        return velocity;
                    }
                    
                case AIState.Falling:
                    {
                        if (_tick != 0 && velocity.Y == 0f && CanStandOn(base.State, roundX, roundY))
                        {
                            TickNextMove(position);
                            return GetVelocity(position, velocity);
                        }
                        _tick++;
                        float diff = _activeMove.XTarget * 16f - position.X + 8f;
                        if (diff > 0)
                        {
                            velocity.X = _moveSpeed * 16f;
                        }
                        else
                        {
                            velocity.X = -_moveSpeed * 16f;
                        }
                        return velocity;
                    }
                    
            }
            State = AIState.Idle;
            return velocity;
        }


        #endregion
    }
}
