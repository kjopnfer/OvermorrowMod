using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Content.UI
{
	/**
	 * From Spirit Mod Source
	 */
	public class RewardList : UIElement
	{
		public List<UIElement> _items = new List<UIElement>();

		protected UIScrollbar _scrollbar;

		internal UIElement _innerList = new RewardList.UIInnerList();

		private float _innerListHeight;

		public float ListPadding = 5f;

		public Vector2 ItemSize;

		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		public RewardList()
		{
			_innerList.OverflowHidden = false;
			_innerList.Width.Set(0f, 1f);
			_innerList.Height.Set(0f, 1f);
			OverflowHidden = true;
			Append(_innerList);
		}

		public virtual void Add(UIElement item)
		{
			_items.Add(item);
			_innerList.Append(item);
			UpdateScrollbar();
			_innerList.Recalculate();
		}

		public virtual void AddRange(IEnumerable<UIElement> items)
		{
			_items.AddRange(items);
			foreach (UIElement item in items)
			{
				_innerList.Append(item);
			}
			//UpdateOrder();
			UpdateScrollbar();
			_innerList.Recalculate();
		}

		public virtual void Clear()
		{
			_innerList.RemoveAllChildren();
			_items.Clear();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (_scrollbar != null)
			{
				_innerList.Top.Set(-_scrollbar.GetValue(), 0f);
			}
			Recalculate();
		}

		public override List<SnapPoint> GetSnapPoints()
		{
			SnapPoint snapPoint;
			List<SnapPoint> snapPoints = new List<SnapPoint>();
			if (GetSnapPoint(out snapPoint))
			{
				snapPoints.Add(snapPoint);
			}
			foreach (UIElement _item in _items)
			{
				snapPoints.AddRange(_item.GetSnapPoints());
			}
			return snapPoints;
		}

		public float GetTotalHeight()
		{
			return _innerListHeight;
		}

		public void Goto(UIList.ElementSearchMethod searchMethod)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (searchMethod(_items[i]))
				{
					_scrollbar.ViewPosition = _items[i].Top.Pixels;
					return;
				}
			}
		}

		public override void Recalculate()
		{
			Recalculate();
			UpdateScrollbar();
		}

		public override void RecalculateChildren()
		{
			RecalculateChildren();

			float myWidth = GetDimensions().Width;

			float currentX = 0f;
			float currentY = 0f;
			float height = 0f;
			for (int i = 0; i < _items.Count; i++)
			{
				float intendedX = currentX + ItemSize.X + ListPadding;
				if (height == 0f)
				{
					height = ItemSize.Y + ListPadding;
				}
				if (intendedX >= myWidth)
				{
					currentY += ItemSize.Y + ListPadding;
					height += ItemSize.Y + ListPadding;
					intendedX -= currentX;
					currentX = 0f;
				}

				_items[i].Left.Set(currentX, 0f);
				_items[i].Top.Set(currentY, 0f);
				_items[i].Width.Set(ItemSize.X, 0f);
				_items[i].Height.Set(ItemSize.Y, 0f);
				_items[i].Recalculate();

				currentX = intendedX;
			}

			_innerListHeight = height;
		}


		public virtual bool Remove(UIElement item)
		{
			_innerList.RemoveChild(item);
			UpdateScrollbar();
			return _items.Remove(item);
		}

		public override void ScrollWheel(UIScrollWheelEvent evt)
		{
			ScrollWheel(evt);
			if (_scrollbar != null)
			{
				UIScrollbar viewPosition = _scrollbar;
				viewPosition.ViewPosition = viewPosition.ViewPosition - (float)evt.ScrollWheelValue;
			}
		}

		public void SetScrollbar(UIScrollbar scrollbar)
		{
			_scrollbar = scrollbar;
			UpdateScrollbar();
		}

		public int SortMethod(UIElement item1, UIElement item2)
		{
			return item1.CompareTo(item2);
		}

		public override void Update(GameTime gameTime)
		{
			for (int i = 0; i < Elements.Count; i++)
			{
				Elements[i].Update(gameTime);
			}
		}

		public void UpdateOrder()
		{
			_items.Sort(new Comparison<UIElement>(SortMethod));
		}

		private void UpdateScrollbar()
		{
			if (_scrollbar == null)
			{
				return;
			}
			_scrollbar.SetView(GetInnerDimensions().Height, _innerListHeight);
		}

		public delegate bool ElementSearchMethod(UIElement element);

		private class UIInnerList : UIElement
		{
			public UIInnerList()
			{
			}

			public override bool ContainsPoint(Vector2 point)
			{
				return true;
			}

			protected override void DrawChildren(SpriteBatch spriteBatch)
			{
				Vector2 vector2 = Parent.GetDimensions().Position();
				Vector2 vector21 = new Vector2(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
				for (int i = 0; i < Elements.Count; i++)
				{
					UIElement element = Elements[i];
					Vector2 vector22 = element.GetDimensions().Position();
					Vector2 vector23 = new Vector2(element.GetDimensions().Width, element.GetDimensions().Height);
					if (!Collision.CheckAABBvAABBCollision(vector2, vector21, vector22, vector23))
					{
						continue;
					}
					element.Draw(spriteBatch);
				}
			}
		}
	}
}