using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace moredrygrass.src
{
    class MoreDryGrass : BlockBehavior
    {
        private OrderedDictionary<string, double> extraGrassPercentage = new OrderedDictionary<string, double>();
        private Item drygrass = null;

        private ThreadLocal<bool> withinCheck = new ThreadLocal<bool>(); // see below, I'm not acctually sure if this needs to be thread safe or not, but it seems prudent.
        private static Random random = new Random();

        bool forKnife = false;
        bool forScythe = false;

        public MoreDryGrass(Block block) : base(block)
        {
            forKnife = MoreDryGrassMod.config.for_knives;
            forScythe = MoreDryGrassMod.config.for_scythes;

            extraGrassPercentage.Add("veryshort", MoreDryGrassMod.config.chances_veryshort);
            extraGrassPercentage.Add("short", MoreDryGrassMod.config.chances_short);
            extraGrassPercentage.Add("mediumshort", MoreDryGrassMod.config.chances_mediumshort);
            extraGrassPercentage.Add("medium", MoreDryGrassMod.config.chances_medium);
            extraGrassPercentage.Add("tall", MoreDryGrassMod.config.chances_tall);
            extraGrassPercentage.Add("verytall", MoreDryGrassMod.config.chances_verytall);
        }

        public override void OnLoaded(ICoreAPI api)
        {
            drygrass = api.World.GetItem(new AssetLocation("game", "drygrass"));
        }

        public override void OnUnloaded(ICoreAPI api)
        {
            drygrass = null;
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropChanceMultiplier, ref EnumHandling handling)
        {
            ItemStack[] rv = new ItemStack[0];

            if (byPlayer != null && drygrass != null)
            {
                bool enabled = (forScythe && byPlayer.InventoryManager.ActiveTool == EnumTool.Scythe) ||
                   ( forKnife && byPlayer.InventoryManager.ActiveTool == EnumTool.Knife );

                if (enabled && !withinCheck.Value) // see below...
                {
                    Block blk = world.BlockAccessor.GetBlock(pos);

                    bool supportsTallgrass = blk.Variant["tallgrass"] != null;
                    if (supportsTallgrass) // no such thing?
                    {
                        bool canHaveExtra = extraGrassPercentage.Keys.Contains(blk.Variant["tallgrass"]);
                        if (canHaveExtra) // unsupported grass size?
                        {
                            double moreGrassPercent = extraGrassPercentage[blk.Variant["tallgrass"]];

                            if (random.NextDouble() < moreGrassPercent)
                            {
                                /*
                                    * This is about to get kinda dumb,
                                    * because of the way GetDrops either uses behaviors, or dosn't we need to work around it.
                                    * 
                                    * We can't just "insert a new grass" into the default loot definition, so lets instead call it.
                                    * 
                                    * BUT WAIT if we do that this code will execute twice! ( yep, this is why we prevent recursion )
                                    * 
                                    * Then, when we have the default result, we'll adjust it
                                    */

                                try
                                {
                                    withinCheck.Value = true;
                                    rv = blk.GetDrops(world, pos, byPlayer, dropChanceMultiplier);
                                }
                                finally
                                {
                                    withinCheck.Value = false;
                                }

                                for (int x = 0; x < rv.Length; ++x)
                                {
                                    if (rv[x].Item == drygrass)
                                    {
                                        rv[x].StackSize += moreGrassPercent > 1 ? ( random.NextDouble() < moreGrassPercent - 1 ? 2 : 1 ) : 1; // for good measure add a third one for over 1 values.
                                        break; // we are only interested in adjusting 1 instance, of drygrass.
                                    }
                                }

                                handling = EnumHandling.PreventDefault;
                            }
                        }
                    }
                }
            }

            return rv;
        }

    }
}
