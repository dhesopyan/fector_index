@ModelType Fector_Index.DashboardViewModel

@Code
    ViewData("Title") = "Home"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    
    Dim CurrencyList As IEnumerable(Of SelectListItem) = ViewBag.CurrencyList
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim BranchList As IEnumerable(Of SelectListItem) = ViewBag.BranchList
End Code

<div class="row col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>EXCHANGE RATE MOVEMENTS</h4>
            </div>
            <div class="pull-right">
                <div class="col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.ExRateCurr)</label>
                    @Html.DropDownListFor(Function(f) f.ExRateCurr, CurrencyList, New With {.class = "form-control", .placeholder = "Currency", .TextAlign = "justify"})
                    @Html.ValidationMessageFor(Function(f) f.ExRateCurr, String.Empty, New With {.class = "help-block"})
                </div>
              </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <div id="divExchangeRate" style="height: 250px;"></div>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>TOTAL DEAL</h4>
            </div>
            <div class="pull-right">
                <div class="col-sm-6">
                    <label>@Html.DisplayNameFor(Function(f) f.StartPeriod)</label>
                    <div class="form-inline">
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
                <div class="col-sm-3">
                    <div class="text-left">
                        <label>@Html.DisplayNameFor(Function(f) f.ExRateDeal)</label>
                        @Html.DropDownListFor(Function(f) f.ExRateDeal, CurrencyList, New With {.class = "form-control", .placeholder = "Currency"})
                        @Html.ValidationMessageFor(Function(f) f.ExRateDeal, String.Empty, New With {.class = "help-block"})
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="text-left">
                    <label>@Html.DisplayNameFor(Function(f) f.DealBranch)</label>
                    @Html.DropDownListFor(Function(f) f.DealBranch, BranchList, New With {.class = "form-control", .placeholder = "Branch"})
                    @Html.ValidationMessageFor(Function(f) f.DealBranch, String.Empty, New With {.class = "help-block"})
                </div>
               </div>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <div id="divDeal" style="height: 250px;"></div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="row col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>TOTAL TRANSACTION</h4>
            </div>
            <div class="pull-right">
                <div class="col-sm-6">
                    <div class="text-left">
                        <label>@Html.DisplayNameFor(Function(f) f.ExRateTrans)</label>
                        @Html.DropDownListFor(Function(f) f.ExRateTrans, CurrencyList, New With {.class = "form-control", .placeholder = "Currency"})
                        @Html.ValidationMessageFor(Function(f) f.ExRateTrans, String.Empty, New With {.class = "help-block"})
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="text-left">
                        <label>@Html.DisplayNameFor(Function(f) f.TransactionBranch)</label>
                        @Html.DropDownListFor(Function(f) f.TransactionBranch, BranchList, New With {.class = "form-control", .placeholder = "Branch"})
                        @Html.ValidationMessageFor(Function(f) f.TransactionBranch, String.Empty, New With {.class = "help-block"})
                    </div>
                </div>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <div id="divTrans" style="height: 250px;"></div>
                </div>
            </div>
        </div>
    </div>
</div>
@Section scripts
    <script>
        var baseUrl = '@Url.Content("~")';

        $(function () {
            $('#ExRatePeriod').datepicker({
                dateFormat: "dd-mm-yy",
            });

            var d = new Date();

            var month = d.getMonth() + 1;
            var day = d.getDate();

            var output = (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + d.getFullYear();

            $('#ExRatePeriod').val(output);

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

    @Scripts.Render("~/Scripts/dashboard.js")
End Section
