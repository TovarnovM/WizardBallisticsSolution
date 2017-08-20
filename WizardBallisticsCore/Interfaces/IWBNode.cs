using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics {
    public interface IWBNode {
        IWBNode Clone();
        T Clone<T>();
    }
}
