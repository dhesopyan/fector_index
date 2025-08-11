@ModelType Fector_Index.ExtractDataManualViewModel 

@Code
    ViewData("Title") = "Extract Data Manual"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim CountDealProfit As Integer = 0
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "ExtractDataManual", FormMethod.Post, New With {.class = "panel panel-default"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Extract Data Manual</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.TDate)</label>
                    <div class="form-group">
                        @Html.CustomTextBoxFor(Function(f) f.TDate, New With {.class = "form-control", .placeholder = "Start Period", .style = "text-transform:uppercase"})
                        @Html.ValidationMessageFor(Function(f) f.TDate, String.Empty, New With {.class = "help-block"})
                        <span class="help-block">This function can only be use if server stuck and the download process can not be done</span>
                    </div>
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnExtract" class="btn btn-primary"><i class="fa fa-download"></i> Extract</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExtractDataManual")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using

    @Section scripts
        <script>
            $(function () {
                $('#TDate').datepicker({
                    dateFormat: "dd-mm-yy",
                });

                var d = new Date();

                var month = d.getMonth() + 1;
                var day = d.getDate();

                var output = (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + d.getFullYear();

                $('#TDate').val(output);
            });
        </script>
    End Section
</div>
