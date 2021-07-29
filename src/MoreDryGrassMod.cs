using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace moredrygrass.src
{
    class MoreDryGrassMod : ModSystem
    {
        public static MoreDryGrassConfig config;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            try
            {
                config = api.LoadModConfig<MoreDryGrassConfig>("moredrygrass.json");
                if (config == null) throw new FileNotFoundException();
            }
            catch (Exception)
            {
                config = new MoreDryGrassConfig();
                api.StoreModConfig<MoreDryGrassConfig>(config, "moredrygrass.json");
            }

            api.RegisterBlockBehaviorClass("moredrygrass:moredrygrass", typeof(MoreDryGrass));
        }
    }
}
