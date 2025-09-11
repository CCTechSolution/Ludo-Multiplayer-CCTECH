using UnityEngine;

namespace HK.Core
{
  public class ApplicationManagerLoader : IAutoLoadable
  {
    static ApplicationManagerLoader()
    {
      new GameObject(typeof(ApplicationManager).Name, typeof(ApplicationManager));
    }
  }
}
