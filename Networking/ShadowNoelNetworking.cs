using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XX;

namespace WeNeedMoreNoels.Networking
{
    public class ShadowNoelNetworking : NetworkBehaviour
    {
        [Networked]
        public bool walk { get; set; }
        private bool _walk;

        [Networked]
        public AIM Aim { get; set; }

        public override void FixedUpdateNetwork()
        {
            if (walk)
            {
                if (!_walk)
                {
                    ShadowNoelNetworkExtensions.Walk(GetComponent<ShadowNoel>());
                    _walk = true;
                }
            }
            else
            {
                if (_walk)
                {

                    _walk = false;
                }
            }
        }
    }
}
