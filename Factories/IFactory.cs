using blackbelt.Models;
using System.Collections.Generic;

namespace blackbelt.Factory {

    public interface IFactory<T> where T: BaseEntity {}
}