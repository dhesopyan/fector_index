@ModelType Fector_Index.ChangePasswordViewModel
@Code
    ViewData("Title") = "Change Password"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<div class="col-md-12">
@Using Html.BeginForm("ChangePassword", "User", FormMethod.Post, New With {.defaultbutton = "BtnSubmit", .class = "panel panel-default"})
    @Html.AntiForgeryToken
    @<div class="panel-heading">
        <div class="pull-left">
            <h4>Password Information</h4>
        </div>
        <div class="clearfix"></div>
    </div>
    @<div class="panel-body">
        <div class="form-group">
            <label>@Html.DisplayNameFor(Function(f) f.OldPassword)</label>
            @Html.CustomPasswordFor(Function(f) f.OldPassword, New With {.class = "form-control", .placeholder = "Old Password"})
            @Html.ValidationMessageFor(Function(f) f.OldPassword, String.Empty, New With {.class = "help-block"})
        </div>
        <div class="form-group">
            <label>@Html.DisplayNameFor(Function(f) f.NewPassword)</label>
            @Html.CustomPasswordFor(Function(f) f.NewPassword, New With {.class = "form-control", .placeholder = "New Password"})
            @Html.ValidationMessageFor(Function(f) f.NewPassword, String.Empty, New With {.class = "help-block"})
        </div>
        <div class="form-group">
            <label>@Html.DisplayNameFor(Function(f) f.ConfirmNewPassword)</label>
            @Html.CustomPasswordFor(Function(f) f.ConfirmNewPassword, New With {.class = "form-control", .placeholder = "Confirm New Password"})
            @Html.ValidationMessageFor(Function(f) f.ConfirmNewPassword, String.Empty, New With {.class = "help-block"})
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
    <script type="text/javascript">
        
    </script>
</div>     