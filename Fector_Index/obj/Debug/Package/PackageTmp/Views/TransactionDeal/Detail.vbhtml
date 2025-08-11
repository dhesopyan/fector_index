@ModelType Fector_Index.TransactionDeal 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim Branch As SelectList = ViewBag.Branch
    Dim CurrencyNoIDR As IEnumerable(Of SelectListItem) = ViewBag.CurrencyNoIDR
    Dim Currency As IEnumerable(Of SelectListItem) = ViewBag.Currency
    Dim Status As IEnumerable(Of SelectListItem) = ViewBag.Status
    Dim RateTypeOption As IEnumerable(Of SelectListItem) = ViewBag.RateTypeOption
    Dim TransactionTypeOption As IEnumerable(Of SelectListItem) = ViewBag.TransactionTypeOption
    Dim DealPeriodOption As IEnumerable(Of SelectListItem) = ViewBag.DealPeriodOption
    Dim BranchLoginUser As String = Session("BranchId")
    Dim BranchHO As String = Session("BranchHO")
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "TransactionDeal", FormMethod.Post, New With {.class = "panel panel-default", .id = "frmData"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Transaction Deal Information</h4>
            </div>
            <div class="pull-right">
                <a href="@Url.Action("BrowseDeal", "TransactionDeal")" class="modal-link btn btn-primary" id="btnsearchdeal" title="Browse"><i class='fa fa-plus'></i> History Deal</a>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-6 hidden">
                    <label>@Html.DisplayNameFor(Function(f) f.ID)</label>
                    @Html.CustomTextBoxFor(Function(f) f.ID, New With {.class = "form-control", .placeholder = "Deal Number", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.ID, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DealNumber)</label>
                    @Html.CustomTextBoxFor(Function(f) f.DealNumber, New With {.class = "form-control", .placeholder = "Deal Number", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.DealNumber, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.BranchId)</label>
                    @Html.DropDownListFor(Function(f) f.BranchId, Branch, "-- SELECT BRANCH --", New With {.class = "form-control", .placeholder = "Branch"})
                    @*@Html.ValidationMessageFor(Function(f) f.BranchId, String.Empty, New With {.class = "help-block"})*@
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.AccNum)</label>
                    <div class="input-group">
                    @Html.CustomTextBoxFor(Function(f) f.AccNum, New With {.class = "form-control", .placeholder = "Account Number"})
                        <span class="input-group-btn">
                            <a href="#" class="modal-link btn btn-primary" id="btnclearaccnum" title="Clear"><i class='fa fa-refresh'></i></a>
                            <a href="@Url.Action("BrowseAccNum", "TransactionDeal")" class="modal-link btn btn-primary" id="btnsearchaccnum" title="Browse"><i class='fa fa-search'></i></a>
                        </span>
                    </div>
                    @Html.ValidationMessageFor(Function(f) f.AccNum, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.AccName)</label>
                    @Html.CustomTextBoxFor(Function(f) f.AccName, New With {.class = "form-control", .placeholder = "Account Name"})
                    @Html.ValidationMessageFor(Function(f) f.AccName, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TransactionType)</label>
                    @Html.DropDownListFor(Function(f) f.DealType, RateTypeOption, New With {.class = "form-control", .placeholder = "Rate Type"})
                    @Html.ValidationMessageFor(Function(f) f.DealType, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DealType)</label> 
                    @Html.DropDownListFor(Function(f) f.TransactionType, TransactionTypeOption, New With {.class = "form-control", .placeholder = "Transaction Type"})
                    @Html.ValidationMessageFor(Function(f) f.TransactionType, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.CurrencyDeal )</label>
                    @Html.DropDownListFor(Function(f) f.CurrencyDeal, CurrencyNoIDR, New With {.class = "form-control", .placeholder = "Base Currency"})
                    @Html.ValidationMessageFor(Function(f) f.CurrencyDeal, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.AmountDeal)</label>
                    @Html.CustomTextBoxFor(Function(f) f.AmountDeal, New With {.class = "form-control autoNumeric", .placeholder = "Base amount"})
                    @Html.ValidationMessageFor(Function(f) f.AmountDeal, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.CurrencyCustomer)</label>
                     @Html.DropDownListFor(Function(f) f.CurrencyCustomer, Currency, New With {.class = "form-control", .placeholder = "Counter Currency"})
                     @Html.ValidationMessageFor(Function(f) f.CurrencyCustomer, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.AmountCustomer)</label>
                     @Html.CustomTextBoxFor(Function(f) f.AmountCustomer, New With {.class = "form-control autoNumeric", .placeholder = "Counter Amount"})
                     @Html.ValidationMessageFor(Function(f) f.AmountCustomer, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DealRate)</label>
                    @Html.CustomTextBoxFor(Function(f) f.DealRate, New With {.class = "form-control autoNumericRate", .placeholder = "Base Rate"})
                    @Html.ValidationMessageFor(Function(f) f.DealRate, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6 hidden">
                    <label>@Html.DisplayNameFor(Function(f) f.RateCustomer)</label>
                    @Html.CustomTextBoxFor(Function(f) f.RateCustomer, New With {.class = "form-control autoNumeric", .placeholder = "Counter Rate"})
                    @Html.ValidationMessageFor(Function(f) f.RateCustomer, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DealPeriod)</label>
                    @Html.DropDownListFor(Function(f) f.DealPeriod, DealPeriodOption, New With {.class = "form-control", .placeholder = "Value Period"})
                    @Html.ValidationMessageFor(Function(f) f.DealPeriod, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DealDate)</label>
                    <input type="text" class="form-control" placeholder="ValueDate" id="viewDealDate" />
                    @Html.CustomTextBoxFor(Function(f) f.DealDate, New With {.class = "form-control hidden", .placeholder = "Value Date"})
                    @Html.ValidationMessageFor(Function(f) f.DealDate, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                @If (Action = "Process") Then
                    @<button name="approvalButton" class="btn btn-primary" value="Approve"><i class="fa fa-check"></i> Approve</button>
                    @<button name="approvalButton" class="btn btn-primary" value="Reject"><i class="fa fa-close"></i> Reject</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Approval", "TransactionDeal")"><i class="fa fa-arrow-left"></i> Back</a>
                ElseIf (Action = "Create") or (Action = "Edit") Then
                    @<button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "TransactionDeal")"><i class="fa fa-arrow-left"></i> Back</a>
                ElseIf (Action = "Viewed") Then
                    @<button name="deleteButton" class="btn btn-primary" value="Delete"><i class="fa fa-trash"></i> Delete</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "TransactionDeal")"><i class="fa fa-arrow-left"></i> Back</a>
                Else
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("History", "TransactionDeal")"><i class="fa fa-arrow-left"></i> Back</a>
                End If
            </div>
            <div class="clearfix"></div>
        </div>
        @<style>
             .modal-content {
                 position: fixed;
                 left: 0px;
                 right: 0px;
                 width: 800px !important;
                 height: auto !important;
                 margin: auto auto !important;
             }
        </style>
        @<div id="modal-container" class="modal fade" tabindex="-1" role="dialog">
            <div class="modal-content">
            </div>
        </div>
    End Using
    @Section scripts
        <script>
            var firstview = true;
            $(document).ready(function () {
                $('#DealNumber').attr('readonly', 'readonly');
                $('#AccNum').attr('readonly', 'readonly');
                $('#AmountCustomer').attr('readonly', 'readonly');

                if ('@BranchLoginUser' == '@BranchHO') {
                    $('#BranchId').val('@BranchHO');
                    $('#BranchId').select2().select2('val', '@BranchHO');
                    BranchId.disabled = false;
                } else {
                    $('#BranchId').val('@BranchLoginUser');
                    $('#BranchId').select2().select2('val', '@BranchLoginUser');
                    BranchId.disabled = true;
                }

                if ('@Action' == 'Process' || '@Action' == 'Viewed' || '@Action' == 'ViewHistory') {
                    $('#AccName').attr('readonly', 'readonly');
                    $('#btnsearchdeal').attr('disabled', 'disabled');
                    $('#btnsearchaccnum').attr('disabled', 'disabled');
                    $('#btnclearaccnum').attr('disabled', 'disabled');
                    DealType.disabled = true;
                    BranchId.disabled = true;
                    TransactionType.disabled = true;
                    CurrencyDeal.disabled = true;
                    $('#DealRate').attr('readonly', 'readonly');
                    $('#AmountDeal').attr('readonly', 'readonly');
                    CurrencyCustomer.disabled = true;
                    DealPeriod.disabled = true;
                    $('#DealDate').attr('readonly', 'readonly');
                }
            else {
            if ('@Action' == 'Create') {
                firstview = false;
            }
            $('#AccName').attr('readonly', false);
            $('#btnsearchaccnum').attr('disabled', false);
            $('#btnclearaccnum').attr('disabled', false);
            DealType.disabled = false;
            TransactionType.disabled = false;
            CurrencyDeal.disabled = false;
            $('#DealRate').attr('readonly', false);
            $('#AmountDeal').attr('readonly', false);
            CurrencyCustomer.disabled = false;
            DealPeriod.disabled = false;
            $('#DealDate').attr('readonly', false);
            }

            var d;

            if (firstview == true){
                d = new Date( $("#DealDate").val());
            }
            else{
                d = new Date();
                checkValueDate();
            }

            var month = d.getMonth() + 1;
            var day = d.getDate();

            var output = (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + d.getFullYear();

            calculateConvertionWithGetRate($("#TransactionType").val(), $("#DealType").val(), $("#CurrencyDeal").val(), $("#CurrencyCustomer").val(), $("#AmountDeal").val());
            $('#viewDealDate').val(output);
            $('#viewDealDate').attr('readonly', 'readonly');
            $('#DealDate').val(output);
            $('#DealDate').attr('readonly', 'readonly');
            firstview = false;
            });

            $("#DealRate, #AmountDeal").change(function () {
                calculateConvertion($("#TransactionType").val(), $("#DealType").val(), $("#CurrencyDeal").val(), $("#CurrencyCustomer").val(), $("#AmountDeal").val(), $("#DealRate").val());
            });

            $("#TransactionType, #DealType, #CurrencyDeal, #CurrencyCustomer").change(function () {
                calculateConvertionWithGetRate($("#TransactionType").val(), $("#DealType").val(), $("#CurrencyDeal").val(), $("#CurrencyCustomer").val(), $("#AmountDeal").val());
            });

            $("#CurrencyDeal").change(function () {
                checkValueDate();
            });

            function calculateConvertionWithGetRate(tranType, rateType, fromCurr, toCurr, amount) {
                if (firstview == false){
                    $.ajax({
                        type: 'POST',
                        url: '@Url.Action("JsOnGetRate")',
                        data: { tranType: tranType, rateType: rateType, fromCurr: fromCurr, toCurr: toCurr },
                        cache: false,
                        success: function (data) {
                            newexrate = data.exchangeRate;
                            $("#DealRate").val(parseFloat(newexrate).toLocaleString("en-US"));
                            calculateConvertion(tranType, rateType, fromCurr, toCurr, amount, String(newexrate));
                        }
                    });
                }
            };

            function calculateConvertion(tranType, rateType, fromCurr, toCurr, amount, rate) {
                var convAmount = 0.0;
                if (!rate) {
                    convAmount = 0.0;
                }
                else {
                    if (!amount) {
                        convAmount = 0.0;
                    }
                    else {
                        convAmount = parseFloat(amount.replaceAll(",", "")) * parseFloat(rate.replaceAll(",", ""));
                    }
                }
                $("#AmountCustomer").val(parseFloat(convAmount).toLocaleString("en-US"))
            };

            function checkValueDate() {
                var valueType = $('#DealPeriod').val();
                var basecurr = $('#CurrencyDeal').val();

                $.ajax({
                    url: 'JsOnGetValueDate',
                    type: 'post',
                    dataType: 'json',
                    data: { valuetype: valueType, basecurrency: basecurr },
                    success: function (data) {
                        $('#viewDealDate').val(data.valueDate);
                        $('#DealDate').val(data.valueDate);
                        if (valueType == "TOD") {
                            try {
                                $("#viewDealDate").datepicker("destroy");
                            }
                            catch (err) { }
                            $("#viewDealDate").attr("style", "cursor:not-allowed")
                        }
                        else if (valueType != "FWD") {
                            $("#viewDealDate").datepicker({
                                dateFormat: "dd-mm-yy",
                            });
                            try {
                                $("#viewDealDate").datepicker("destroy");
                            }
                            catch (err) { }
                            $("#viewDealDate").attr("style", "cursor:not-allowed")
                        }
                        else {
                            $('#viewDealDate').datepicker({
                                dateFormat: "dd-mm-yy",
                            });
                            $('#viewDealDate').attr('readonly', false);
                        }
                    }
                });
            }

            String.prototype.replaceAll = function (str1, str2, ignore) {
                return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
            };

            $('#BtnSubmit').click(function () {
                var selectedDescription = $('#DealPeriod').val();
                var selectedDate = $('#viewDealDate').val();

                //var d = new Date(selectedDescription);

                //var month = d.getMonth() + 1;
                //var day = d.getDate();

                //if (selectedDescription == "TOD") {
                //    $('#DealDate').attr('readonly', 'readonly');
                //    $('#DealDate').val(selectedDate.substring(6,4) + '-' + selectedDate.substring(3,2) + '-' + selectedDate.substring(0,2));
                //}
                //else if (selectedDescription == "TOM") {
                //    var day = d.getDay() + 1;
                //    $('#DealDate').val(d.getFullYear() + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day);
                //    $('#DealDate').attr('readonly', false);
                //}
                //else if (selectedDescription == "SPOT") {
                //    var day = d.getDay() + 2;
                //    $('#DealDate').val(d.getFullYear() + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day);
                //    $('#DealDate').attr('readonly', false);
                //}
                $('#DealDate').val(selectedDate.substring(6, 10) + '-' + selectedDate.substring(3, 5) + '-' + selectedDate.substring(0, 2));
            });

            $('#DealPeriod').change(function () {
                checkValueDate();
            });

            $(function () {
                $('#viewDealDate').datepicker({
                    dateFormat: "dd-mm-yy",
                });
                $('#DealDate').datepicker({
                    dateFormat: "dd-mm-yy",
                });
                try {
                    $("#viewDealDate").datepicker("destroy");
                }
                catch (err) { }
                $("#viewDealDate").attr("style", "cursor:not-allowed")

                $('body').on('click', '#btnsearchaccnum', function (e) {
                    e.preventDefault();
                    var link = $(this).attr('href');
                    $(this).attr('href', link);
                    $(this).attr('data-target', '#modal-container');
                    $(this).attr('data-toggle', 'modal');
                    $('#AccNum').val("");
                    $('#AccName').val("");
                    $('#AccNum').attr('readonly', 'readonly');
                    $('#AccName').attr('readonly', 'readonly');
                });
                $('body').on('click', '#btnsearchdeal', function (e) {
                    e.preventDefault();
                    var link = $(this).attr('href');
                    $(this).attr('href', link);
                    $(this).attr('data-target', '#modal-container');
                    $(this).attr('data-toggle', 'modal');
                });
                $('body').on('click', '#btnclearaccnum', function (e) {
                    e.preventDefault();
                    $('#AccNum').val("");
                    $('#AccName').val("");
                    $('#AccNum').attr('readonly', 'readonly');
                    $('#AccName').attr('readonly', false);
                });
                $('#modal-container').on('hidden.bs.modal', function () {
                    $(this).removeData('bs.modal');
                    $('#btnsearchaccnum').attr('href', '@Url.Action("BrowseAccNum", "TransactionDeal")');
                });
            });
        </script>
    End Section
</div>
