@ModelType Fector_Index.RptDailyProfit 
@Code
    ViewData("Title") = "Report Daily Profit"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code

<div class="col-md-12">
    @Using Html.BeginForm("RptDailyProfit", "Report", FormMethod.Post, New With {.class = "panel panel-default"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Report Daily Profit</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.Period)</label>
                    <div class="form-inline ">
                        <div class="form-group">
                            @Html.CustomTextBoxFor(Function(f) f.Period, New With {.class = "form-control", .placeholder = "Start Period", .style = "text-transform:uppercase"})
                            @Html.ValidationMessageFor(Function(f) f.Period, String.Empty, New With {.class = "help-block"})
                        </div>
                    </div>
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnPrint" class="btn btn-primary"><i class="fa fa-print"></i> Print</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("IndexDailyProfit", "Report")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using

    @Section scripts
        <script>
            $(function () {
                $('#Period').datepicker({
                    changeMonth: true,
                    changeYear: true,
                    showButtonPanel: true,
                    dateFormat: 'mm-yy',
                    onClose: function (dateText, inst) {
                        $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1));
                    }
                });

                var d = new Date();

                var month = d.getMonth() + 1;
                var day = d.getDate();

                var output = (month < 10 ? '0' : '') + month + '-' + d.getFullYear();

                $('#Period').val(output);

            });
        </script>
        <style>
            .ui-datepicker-calendar {
                display: none;
            }
        </style>
    End Section
</div>
