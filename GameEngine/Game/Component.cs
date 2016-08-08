using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public abstract class Component
    {
        internal GameObject _parent;

        public virtual void Dispose()
        {

        }
        public virtual void Start()
        {

        }
        public virtual void Update()
        {

        }
        public virtual void Draw()
        {

        }
    }
}
