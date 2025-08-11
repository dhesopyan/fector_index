@ModelType Fector_Index.MsSettingSecurityViewModel 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code

<div class="col-md-12">
    @Using Html.BeginForm("Security", "Setting", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Security Setting Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.LoginTimeout)</label>
                    @Html.CustomTextBoxFor(Function(f) f.LoginTimeout, New With {.class = "form-control", .placeholder = "Login Timeout"})
                    @Html.ValidationMessageFor(Function(f) f.LoginTimeout, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.MinPassword)</label>
                    @Html.CustomTextBoxFor(Function(f) f.MinPassword, New With {.class = "form-control", .placeholder = "Minimum Password"})
                    @Html.ValidationMessageFor(Function(f) f.MinPassword, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.MaxPassword)</label>
                    @Html.CustomTextBoxFor(Function(f) f.MaxPassword, New With {.class = "form-control", .placeholder = "Maximum Password"})
                    @Html.ValidationMessageFor(Function(f) f.MaxPassword, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.MaxIncorrect)</label>
                    @Html.CustomTextBoxFor(Function(f) f.MaxIncorrect, New With {.class = "form-control", .placeholder = "Maximum Incorrect Password"})
                    @Html.ValidationMessageFor(Function(f) f.MaxIncorrect, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.PasswordExpired )</label>
                    @Html.CustomTextBoxFor(Function(f) f.PasswordExpired, New With {.class = "form-control", .placeholder = "Password Expired"})
                    @Html.ValidationMessageFor(Function(f) f.PasswordExpired, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "Home")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
</div>
