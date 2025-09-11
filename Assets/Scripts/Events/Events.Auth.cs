using UnityEngine.Events;

namespace HK
{
  public partial class Events
  {
        public static Event RequestLoginOptions;
        public static Event RequestIdLogin;
        public static Event<string> RequestIdRegisterFailed;
        public static Event<string> RequestIdLoginFailed;
        public static Event<string> RequestOnResetPassword;
        public static Event RequestNameChange;
        public static Event<string, string, UnityAction> RequestErrorPopup;
    }
}
