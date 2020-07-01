namespace WebApi.Controllers.V2
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v{version:apiVersion}";
        public const string Base = Root + "/" + Version;

        public static class Users
        {
            public const string SendCode = Base + "/users/sendcode";
            public const string Login = Base + "/users/login";
            public const string RefreshToken = Base + "/users/refresh";
        }
    }
}
