using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal interface IDamageable
    {
        void TakeDamage(int amount);
        bool IsAlive { get; }
    }
}
