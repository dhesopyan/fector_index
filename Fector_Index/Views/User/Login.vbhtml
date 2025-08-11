@ModelType Fector_Index.LoginViewModel
@Code
    Layout = nothing
    ViewData("Title") = "Fector"
End Code
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <title>Login - Fector</title>
        <meta http-equiv="X-UA-Compatible" content="IE=edge" />

        <!-- Tell the browser to be responsive to screen width -->
        <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" name="viewport" />

        <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
        <meta name="viewport" content="width=device-width" />
        @Styles.Render("~/Content/css")

        <!-- Font Awesome -->
        @*<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.4.0/css/font-awesome.min.css" />*@
        <!-- Ionicons -->
        @*<link rel="stylesheet" href="https://code.ionicframework.com/ionicons/2.0.1/css/ionicons.min.css" />*@
        @Styles.Render("~/Content/font-awesome.min.css")
        @Styles.Render("~/Content/ionicons.min.css")
        @Styles.Render("~/AdminLTE/css")
    </head>
    <body class="hold-transition login-page">
        @Using Html.BeginForm("Login", "User", FormMethod.Post)
            @Html.AntiForgeryToken
            @<div class="login-box">
                <div class="login-logo">
                    <img src="~/Content/Images/logo.png" />
                    <h2 style="font-size:18pt">
                        Foreign Exchange Control and Reporting<br /><br />
                        <b>(FECTOR)</b>
                    </h2>
                </div>
                <!-- /.login-logo -->
                <div class="login-box-body">
                    <form>
                        <div class="form-group has-feedback">
                            <span class="form-control-feedback"><i class="fa fa-user"></i></span>
                            @Html.CustomTextBoxFor(Function(f) f.Username, New With {.class = "form-control", .placeholder = "Username"})
                        </div>
                        <div class="form-group has-feedback">
                            <span class="form-control-feedback"><i class="fa fa-lock"></i></span>
                            @Html.CustomPasswordFor(Function(f) f.Password, New With {.class = "form-control", .placeholder = "Password"})
                        </div>
                        <div class="row">
                            <!-- /.col -->
                            <div class="col-md-4 col-md-offset-8 ">
                                <input type="submit" id="BtnLogin" class="btn btn-primary" value="Login" style="width:100%" />
                            </div>
                            <!-- /.col -->
                        </div>
                        <div class="row">
                            <!-- /.col -->
                            <div class="col-md-4 col-md-offset-8 ">
                                v.20170712
                            </div>
                            <!-- /.col -->
                        </div>
                        @Html.HiddenFor(Function(f) f.ReturnUrl)
                    </form>
                </div>
                <!-- /.login-box-body -->
            </div>
        End Using 
        @Scripts.Render("~/bundles/jquery")
        @Scripts.Render("~/bundles/mainjs")
        @Scripts.Render("~/AdminLTE/js")
        <script>
            var message = "@ViewBag.ErrorMessage";
            if (message.length > 0)
                bootbox.alert(message);
        </script>
    </body>
</html>