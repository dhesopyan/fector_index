@ModelType Fector_Index.MsUser
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim UserLevel As SelectList = ViewBag.UserLevel
    Dim Branch As SelectList = ViewBag.Branch
    Dim Currency As SelectList = ViewBag.Currency
    Dim DefaultPass As String = ViewBag.DefaultPass
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "User", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
    @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>User Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.UserId)</label>
                    @Html.CustomTextBoxFor(Function(f) f.UserId, New With {.class = "form-control", .placeholder = "Username"})
                    @Html.ValidationMessageFor(Function(f) f.UserId, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.Fullname)</label>
                    @Html.CustomTextBoxFor(Function(f) f.Fullname, New With {.class = "form-control", .placeholder = "Full Name"})
                    @Html.ValidationMessageFor(Function(f) f.Fullname, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.BranchId)</label>
                    @Html.DropDownListFor(Function(f) f.BranchId, Branch, "-- SELECT BRANCH --", New With {.class = "form-control", .placeholder = "Branch"})
                    @Html.ValidationMessageFor(Function(f) f.BranchId, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.UserlevelID)</label>
                    @Html.DropDownListFor(Function(f) f.UserlevelID, UserLevel, "-- SELECT USER LEVEL --", New With {.class = "form-control", .placeholder = "User Level", .id="UserlevelID"})
                    @Html.ValidationMessageFor(Function(f) f.UserlevelID, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div id="TransactionLimitPanel" class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TransactionLimitCurrency)</label>
                    @Html.DropDownListFor(Function(f) f.TransactionLimitCurrency, Currency, New With {.class = "form-control", .placeholder = "Branch"})
                    @Html.ValidationMessageFor(Function(f) f.TransactionLimitCurrency, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>&nbsp;</label>
                    @Html.CustomTextBoxFor(Function(f) f.TransactionLimit, New With {.class = "form-control autoNumeric", .placeholder = "Limit"})
                    @Html.ValidationMessageFor(Function(f) f.TransactionLimit, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row pull-right" id="DefaultPass">
                <div class="col-md-12">
                    <label>Password Default : @DefaultPass</label>
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "User")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
    @Section scripts
    <script>
    $(document).ready(function () {
        if ('@Action' == 'Create') {
            $("#DefaultPass").prop("hidden", false);
        }
        else {
            $("#DefaultPass").prop("hidden", true);
        }

        if ('@Action' == 'Edit') {
            $('#UserId').attr('readonly', 'readonly');
        }
        $("#TransactionLimitPanel").prop("hidden", true);

        CheckUserLevel($("#UserlevelID").val());

        function CheckUserLevel(id) {
            if (!id) {
                id = -1;
            }

            $.ajax({
                url: '@Url.Action("JSONCheckUserLevelLimit")' + '/' + id,
                type: 'post',
                dataType: 'json',
                success: function (data) {
                    if (data.useLimit) {
                        $("#TransactionLimitPanel").prop("hidden", false);
                        if ('@Action' == 'Create')
                        {
                            $("#TransactionLimit").val("0.00");
                        }
                    }
                    else {
                        $("#TransactionLimitPanel").prop("hidden", true);
                        if ('@Action' == 'Create')
                        {
                            $("#TransactionLimit").val("0.00");
                        }
                    }
                },
            });
        }

        $("#UserlevelID").change(function () {
            CheckUserLevel($(this).val());
        });
    });
    </script>
    End Section
</div>
