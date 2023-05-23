using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OvermorrowMod.Content.Items.Weapons.Magic
{
    public class GlimsporeSpore : ModProjectile
    {
        public int Size = 3;
        public override string Texture => "OvermorrowMod/Content/Items/Weapons/Magic/GlimsporeScepter/GlimsporeSpore";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 60;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 2;
            Projectile.penetrate = -1;
            Projectile.maxPenetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.4f;
        }
        public override bool? CanCutTiles() => false;
        
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, (255 - Projectile.alpha));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
        }

        float RotDir = 0.05f;
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                if (Main.rand.Next(2) == 0)
                    RotDir *= -1;
                Projectile.frame = Main.rand.Next(3);
                if (Main.player[Projectile.owner].name.ToLower().Contains("fg"))
                    Projectile.frame += 3;
            }
            if (Projectile.ai[0] >= 100)
                Projectile.alpha += (int)(255f / 20f);
            Projectile.scale += 0.005f;
            Projectile.rotation += RotDir;
            Projectile.velocity /= 1.05f;
            Projectile.ai[0]++;
        }
    }
    /*public class GlimsporeSpore : ModProjectile
    {
        public override string Texture => "RadiantShadows/Projectiles/ghostProjectile";
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.magic = true;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.scale = 2;
            projectile.penetrate = -1;
            projectile.maxPenetrate = -1;
            projectile.timeLeft = 60;
        }

        List<Projectile> ProjsIOwn = new List<Projectile>();
        public override void AI()
        {
            if (projectile.ai[0] == 0)
                ProjsIOwn.Clear();
            if (projectile.ai[0] % 2 == 0 && projectile.ai[0] > 0)
            {
                Vector2 Pos = projectile.Center + new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-20, 21));
                if (ProjsIOwn.Count < 20)
                    ProjsIOwn.Add(Projectile.NewProjectileDirect(Pos, Vector2.Zero, ModContent.ProjectileType<GlimsporeSporeSpore>(), projectile.damage, 0, projectile.owner, Main.rand.Next(4)));
                else
                    projectile.Kill();
            }
            projectile.ai[0]++;
        }
        public override void Kill(int timeLeft)
        {
            for(int i = 0; ProjsIOwn.Count > i; i++)
                Main.projectile[ProjsIOwn[i].whoAmI].Kill();
        }
    }
    public class GlimsporeSporeSpore : ModProjectile
    {
        public int Size = 3;
        public override string Texture => "RadiantShadows/Projectiles/ghostProjectile";
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.magic = true;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.scale = 2;
            projectile.penetrate = -1;
            projectile.maxPenetrate= -1;
        }
        public override void AI()
        {
            projectile.timeLeft = 240;
            if (projectile.ai[0] == 0)
                Size = 3;
            if (projectile.ai[0] % 2 == 0 && projectile.ai[0] > 0)
                Size--;
            *//*if (projectile.ai[0] % 2 == 0)
                for (int i = 0; Main.rand.Next(1, 3) > i; i++)
                    Dust.NewDustPerfect(projectile.Center + new Vector2(Main.rand.Next(-2, 11), Main.rand.Next(-2, 11)), ModContent.DustType<Dusts.GlimsporeDust>());*//*
            if (0 >= Size)
                Size = 1;
            projectile.ai[0]++;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.Draw(ModContent.GetTexture($"RadiantShadows/Projectiles/GlimsporeSpore{Size}"), projectile.Center - Main.screenPosition, Color.White);
        }
    }*/
}