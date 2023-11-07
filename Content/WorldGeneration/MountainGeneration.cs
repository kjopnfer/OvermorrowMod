// TODO: Saved relevant decompiled WorldGen Mountain Passes for later
/*AddGenerationPass("Mount Caves", delegate (GenerationProgress progress, GameConfiguration passConfig)
{
	numMCaves = 0;
	progress.Message = Lang.gen[2].Value;
	for (int num882 = 0; num882 < (int)((double)Main.maxTilesX * 0.001); num882++)
	{
		progress.Set((float)num882 / (float)Main.maxTilesX * 0.001f);
		int num883 = 0;
		bool flag56 = false;
		bool flag57 = false;
		int num884 = genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
		while (!flag57)
		{
			flag57 = true;
			while (num884 > Main.maxTilesX / 2 - 90 && num884 < Main.maxTilesX / 2 + 90)
			{
				num884 = genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
			}
			for (int num885 = 0; num885 < numMCaves; num885++)
			{
				if (Math.Abs(num884 - mCaveX[num885]) < 100)
				{
					num883++;
					flag57 = false;
					break;
				}
			}
			if (num883 >= Main.maxTilesX / 5)
			{
				flag56 = true;
				break;
			}
		}
		if (!flag56)
		{
			for (int num886 = 0; (double)num886 < Main.worldSurface; num886++)
			{
				if (Main.tile[num884, num886].active())
				{
					for (int num887 = num884 - 50; num887 < num884 + 50; num887++)
					{
						for (int num888 = num886 - 25; num888 < num886 + 25; num888++)
						{
							if (Main.tile[num887, num888].active() && (Main.tile[num887, num888].type == 53 || Main.tile[num887, num888].type == 151 || Main.tile[num887, num888].type == 274))
							{
								flag56 = true;
							}
						}
					}
					if (!flag56)
					{
						Mountinater(num884, num886);
						mCaveX[numMCaves] = num884;
						mCaveY[numMCaves] = num886;
						numMCaves++;
						break;
					}
				}
			}
		}
	}
});

AddGenerationPass("Mountain Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[21].Value;
				for (int num655 = 0; num655 < numMCaves; num655++)
				{
					int i4 = mCaveX[num655];
					int j4 = mCaveY[num655];
					CaveOpenater(i4, j4);
					Cavinator(i4, j4, genRand.Next(40, 50));
				}
			});
*/