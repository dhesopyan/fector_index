@ModelType Fector_Index.RptTransRecapView
@Code
    ViewData("Title") = "Report Transaction Recap"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    @Using Html.BeginForm("RptTransRecap", "Report", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "frmData"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Report Transaction Recap</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.StartPeriod)</label>
                    <div class="form-inline ">
                        <div class="form-group">
                            @Html.CustomTextBoxFor(Function(f) f.StartPeriod, New With {.class = "form-control", .placeholder = "Start Period", .style = "text-transform:uppercase"})
                            @Html.ValidationMessageFor(Function(f) f.StartPeriod, String.Empty, New With {.class = "help-block"})
                        </div>
                        <label>-</label>
                        <div class="form-group">
                            @Html.CustomTextBoxFor(Function(f) f.EndPeriod, New With {.class = "form-control", .placeholder = "End Period", .style = "text-transform:uppercase"})
                            @Html.ValidationMessageFor(Function(f) f.EndPeriod, String.Empty, New With {.class = "help-block"})
                        </div>
                    </div>
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnPrint" class="btn btn-primary"><i class="fa fa-print"></i> Print</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("IndexDeal", "Report")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using

    @Section scripts
        <script>
            $(function () {
                $('#StartPeriod').datepicker({
                    dateFormat: "dd-mm-yy",
                });

                $('#EndPeriod').datepicker({
                    dateFormat: "dd-mm-yy",
                });

                var d = new Date();

                var month = d.getMonth() + 1;
                var day = d.getDate();

                var output = (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + d.getFullYear();

                $('#StartPeriod').val(output);
                $('#EndPeriod').val(output);

            });
        </script>
    End Section
</div>
