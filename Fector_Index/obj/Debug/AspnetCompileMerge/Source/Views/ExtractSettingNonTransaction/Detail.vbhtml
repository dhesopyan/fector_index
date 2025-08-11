@ModelType Fector_Index.MsExtractSettingNonTransaction 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "ExtractSettingNonTransaction", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Extract Setting Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.GLCode )</label>
                    @Html.CustomTextBoxFor(Function(f) f.GLCode, New With {.class = "form-control", .placeholder = "COA", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.GLCode, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TransCode)</label>
                    @Html.CustomTextBoxFor(Function(f) f.TransCode, New With {.class = "form-control", .placeholder = "Transaction Code"})
                    @Html.ValidationMessageFor(Function(f) f.TransCode, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExtractSettingNonTransaction")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
    @Section scripts
        <script>
        $(document).ready(function(){
            if('@Action' == 'Edit')
                {
                    $('#ExtractID').attr('readonly','readonly');
                }
        });
        </script>
    End Section
</div>
