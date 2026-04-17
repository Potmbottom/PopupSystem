using PopupShowcase.PopupSystem;
using R3;

namespace PopupShowcase.Meta.Popups
{
    public class LoginPopupData : BasePopupData
    {
        public override PopupType Type => PopupType.Login;
        public ReactiveProperty<string> PlayerName { get; } = new("");
        public Observable<string> LoginRequested => _loginRequested;

        private readonly Subject<string> _loginRequested = new();

        public LoginPopupData() : base(PopupPriority.Standard)
        {
        }

        public void RequestLogin()
        {
            var name = PlayerName.Value;
            if (string.IsNullOrWhiteSpace(name))
                return;

            _loginRequested.OnNext(name);
        }

        public override void Dispose()
        {
            PlayerName.Dispose();
            _loginRequested.Dispose();
            base.Dispose();
        }
    }
}
