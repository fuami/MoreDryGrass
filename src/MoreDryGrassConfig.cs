using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moredrygrass.src
{
    class MoreDryGrassConfig
    {
        // by default this is a scythe only feature, but you can adjust it to knives, or both, if you do false for both why did you install the mod?
        public bool for_knives = false;
        public bool for_scythes = true;

        // chances are defined as follows,
        // 0.1 is 10% for an extra.
        // 1.0 is 100% for an extra.
        // 1.5 is 100% for one extra, and 50% for a third grass.
        // 2.0 ( the max ) is 3 grass all the time.
        public double chances_veryshort = 0;
        public double chances_short = 0.1;
        public double chances_mediumshort = 0.25;
        public double chances_medium = 0.5;
        public double chances_tall = 1;
        public double chances_verytall = 1.25;
    }
}
