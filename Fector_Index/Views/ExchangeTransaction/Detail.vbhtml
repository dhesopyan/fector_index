@ModelType Fector_Index.ExchangeTransactionViewModel
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim Branch As SelectList = ViewBag.Branch
    Dim CurrencyNoIDR As SelectList = ViewBag.CurrencyNoIDR
    Dim Currency As SelectList = ViewBag.Currency
    Dim Status As SelectList = ViewBag.Status
    Dim LHBUDocumentOption As SelectList = ViewBag.LHBUDocumentOption
    Dim TransactionTypeOption As IEnumerable(Of SelectListItem) = ViewBag.TransactionTypeOption
    Dim RateTypeOption As IEnumerable(Of SelectListItem) = ViewBag.RateTypeOption
    Dim DealPeriodOption As IEnumerable(Of SelectListItem) = ViewBag.DealPeriodOption
    Dim DocumentTransOption As IEnumerable(Of SelectListItem) = ViewBag.DocumentTransOption
    Dim PurposeIdOption As IEnumerable(Of SelectListItem) = ViewBag.PurposeIdOption
    Dim SourceFundsOption As IEnumerable(Of SelectListItem) = ViewBag.SourceFundsOption
    Dim CountListDeal As Integer = 0
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "ExchangeTransaction", FormMethod.Post, New With {.class = "panel panel-default", .id = "frmData", .enctype = "multipart/form-data"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Transaction Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TransNum)</label>
                    @Html.CustomTextBoxFor(Function(f) f.TransNum, New With {.class = "form-control", .placeholder = "Transaction Number", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.TransNum, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6 useDeal">
                    <label>@Html.DisplayNameFor(Function(f) f.DealNumber)</label>
                    <div class="input-group">
                        @Html.CustomTextBoxFor(Function(f) f.DealNumber, New With {.class = "form-control", .placeholder = "Deal Number"})
                        <span class="input-group-btn">
                            <a href="" class="modal-link btn btn-default" id="btnadddeal" name="addButton" value="add" title="Add"><i class='fa fa-plus'></i></a>
                        </span>
                    </div>
                    @Html.ValidationMessageFor(Function(f) f.DealNumber, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row useDeal" id="temp">
                <div class="form-group col-md-12">
                    <table id="TransDetail" class="table FectorTable">
                        <thead>
                            <tr id="0" class="GvHeader">
                                <th width="15%">
                                    Deal Number
                                </th>
                                <th width="15%">
                                    Account Number
                                </th>
                                <th width="20%">
                                    Account Name
                                </th>
                                <th width="10%">
                                    Rate
                                </th>
                                <th width="15%">
                                    Base Amount
                                </th>
                                <th width="15%">
                                    Counter Amount
                                </th>
                                <th width="10%">&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                            @If Not IsNothing(Model.ListOfDeal) Then
                            CountListDeal = 1
                            Dim rowid As Integer = 1
                            Dim id As Integer = rowid - 1
                                @For Each dtl As Fector_Index.TransactionDetail In Model.ListOfDeal
                                    @<tr id="@(CInt(rowid))">
                                         <td>
                                             @dtl.DealNumber
                                             <input type='hidden' id='ListOfDeal.Index' name='ListOfDeal.Index' value='@CInt(id)' />
                                             @Html.HiddenFor(Function(f) f.ListOfDeal(CInt(id)).DealNumber, New With {.class="dealnumber"})  
                                             @Html.HiddenFor(Function(f) f.ListOfDeal(CInt(id)).BaseCurrency)
                                             @Html.HiddenFor(Function(f) f.ListOfDeal(CInt(id)).CounterCurrency)  
                                         </td>
                                         <td>
                                             @dtl.AccNumber
                                             @Html.HiddenFor(Function(f) f.ListOfDeal(CInt(id)).AccNumber)
                                         </td>
                                         <td>
                                             @dtl.AccName
                                             @Html.HiddenFor(Function(f) f.ListOfDeal(CInt(id)).AccName)
                                             <input type="hidden" id="ListOfDeal[@CInt(id)].AccName" , name="ListOfDeal[@CInt(id)].AccName" value="@dtl.AccName" />
                                         </td>
                                         <td>
                                             @dtl.TransRate.ToString("N2")
                                             @Html.HiddenFor(Function(f) f.ListOfDeal(CInt(id)).TransRate)
                                         </td>
                                         <td>
                                             @dtl.BaseNominal.ToString("N2")
                                             @Html.HiddenFor(Function(f) f.ListOfDeal(CInt(id)).BaseNominal, New With {.class = "baseamount"})
                                         </td>
                                         <td>
                                             @dtl.CounterNominal.ToString("N2")
                                             @Html.HiddenFor(Function(f) f.ListOfDeal(CInt(id)).CounterNominal, New With {.class = "counteramount"})
                                         </td>
                                         <td>
                                             <div id="divDeleteDeal">
                                                 @If Action = "Process" Or Action = "Viewed" Then
                                                    @<a href='#' title='Delete' id='DeleteDeal' name="DeleteDeal" data-rowid='@CInt(rowid)' class="hidden"><span class='btn btn-info btn-sm fa fa-trash'></span></a>
                                                 Else
                                                    @<a href='#' title='Delete' id='DeleteDeal' name="DeleteDeal" data-rowid='@CInt(rowid)' ><span class='btn btn-info btn-sm fa fa-trash'></span></a>
                                                 End If
                                             </div>
                                         </td>
                                    </tr>
                                rowid += 1
                                id = rowid - 1
                                Next
                            Else
                            CountListDeal = 0
                            End If
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.AccNum)</label>
                    <div class="input-group">
                        @Html.CustomTextBoxFor(Function(f) f.AccNum, New With {.class = "form-control", .placeholder = "Account Number"})
                        <span class="input-group-btn">
                            <a href="#" class="modal-link btn btn-default" id="btnclearaccnum" title="Clear"><i class='fa fa-refresh'></i></a>
                            <a href="@Url.Action("BrowseAccNum", "ExchangeTransaction")" class="modal-link btn btn-default" id="btnsearchaccnum" title="Browse"><i class='fa fa-search'></i></a>
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
                    @Html.DropDownListFor(Function(f) f.TransactionType, TransactionTypeOption, New With {.class = "form-control", .placeholder = "Transaction Type"})
                    @Html.ValidationMessageFor(Function(f) f.TransactionType, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>&nbsp</label>
                    @Html.DropDownListFor(Function(f) f.RateType, RateTypeOption, New With {.class = "form-control", .placeholder = "Rate Type"})
                    @Html.ValidationMessageFor(Function(f) f.RateType, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TransactionCurrency)</label>
                    @Html.DropDownListFor(Function(f) f.TransactionCurrency, CurrencyNoIDR, New With {.class = "form-control", .placeholder = "Transaction Currency"})
                    @Html.ValidationMessageFor(Function(f) f.TransactionCurrency, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.CustomerCurrency)</label>
                    @Html.DropDownListFor(Function(f) f.CustomerCurrency, Currency, New With {.class = "form-control", .placeholder = "Customer Currency"})
                    @Html.ValidationMessageFor(Function(f) f.CustomerCurrency, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row multirate">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TransactionNominal)</label>
                    @Html.CustomTextBoxFor(Function(f) f.TransactionNominal, New With {.class = "form-control autoNumeric", .placeholder = "Transaction Nominal"})
                    @Html.ValidationMessageFor(Function(f) f.TransactionNominal, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TransactionRate)</label>
                    @Html.CustomTextBoxFor(Function(f) f.TransactionRate, New With {.class = "form-control autoNumeric", .placeholder = "Transaction Rate"})
                    @Html.ValidationMessageFor(Function(f) f.TransactionRate, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
             <div class="row multirate">
                 <div class="col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.CustomerNominal)</label>
                     @Html.CustomTextBoxFor(Function(f) f.CustomerNominal, New With {.class = "form-control autoNumeric", .placeholder = "Convertion Nominal"})
                     @Html.ValidationMessageFor(Function(f) f.CustomerNominal, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.ValuePeriod)</label>
                    @Html.DropDownListFor(Function(f) f.ValuePeriod, DealPeriodOption, New With {.class = "form-control", .placeholder = "Value Period"})
                    @Html.ValidationMessageFor(Function(f) f.ValuePeriod, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.ValueDate)</label>
                    @Html.CustomTextBoxFor(Function(f) f.ValueDate, New With {.class = "form-control", .placeholder = "Value Date"})
                    @Html.ValidationMessageFor(Function(f) f.ValueDate, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.SourceFunds)</label>
                     @Html.DropDownListFor(Function(f) f.SourceFunds, SourceFundsOption, "Select Source Funds", New With {.class = "form-control", .placeholder = "Source of Funds"})
                     @Html.ValidationMessageFor(Function(f) f.SourceFunds, String.Empty, New With {.class = "help-block"})
                 </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.SourceAccNum)</label>
                    <div class="input-group">
                        @Html.CustomTextBoxFor(Function(f) f.SourceAccNum, New With {.class = "form-control", .placeholder = "Source Account Number"})
                        <span class="input-group-btn">
                            <a href="#" class="modal-link btn btn-default" id="btnclearsourceaccnum" title="Clear"><i class='fa fa-refresh'></i></a>
                            <a href="@Url.Action("BrowseSourceAccNum", "ExchangeTransaction")" class="modal-link btn btn-default" id="btnsearchsourceaccnum" title="Browse"><i class='fa fa-search'></i></a>
                        </span>
                    </div>
                    @Html.ValidationMessageFor(Function(f) f.SourceAccNum, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.SourceAccName)</label>
                    @Html.CustomTextBoxFor(Function(f) f.SourceAccName, New With {.class = "form-control", .placeholder = "Source Account Name"})
                    @Html.ValidationMessageFor(Function(f) f.SourceAccName, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row doctrans">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DocumentTransId)</label>
                    @Html.DropDownListFor(Function(f) f.DocumentTransId, DocumentTransOption, "Select Document Transaction", New With {.class = "form-control", .placeholder = "Document Transaction"})
                    @Html.ValidationMessageFor(Function(f) f.DocumentTransId, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row doctrans">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DocumentLHBUId)</label>
                    @Html.DropDownListFor(Function(f) f.DocumentLHBUId, LHBUDocumentOption, "Select LHBU Document", New With {.class = "form-control", .placeholder = "LHBU Document"})
                    @Html.ValidationMessageFor(Function(f) f.DocumentLHBUId, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.PurposeId)</label>
                    @Html.DropDownListFor(Function(f) f.PurposeId, PurposeIdOption, "Select LHBU Purpose",  New With {.class = "form-control", .placeholder = "LHBU Purpose"})
                    @Html.ValidationMessageFor(Function(f) f.PurposeId, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row statementdoc">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.FileDocumentStatement)</label>
                    <div class="input-group">
                        <input class="form-control" placeholder="File Letter of Statement" id="FileDocumentStatementName" />
                        @Html.CustomTextBoxFor(Function(f) f.FileDocumentStatement, New With {.type = "file", .class = "hidden", .onchange = "ChangeText(this, 'FileDocumentStatementName')"})
                        <span class="input-group-btn">
                            <a href="#" class="modal-link btn btn-default" id="btnclearuploaddocstatement" title="Clear"><i class='fa fa-refresh'></i></a>
                            <a href="" onclick="document.getElementById('FileDocumentStatement').click(); return false" class="modal-link btn btn-default" id="btnsearchfiledocstatement" title="Browse"><i class='fa fa-search'></i></a>
                        </span>
                    </div>
                    @Html.ValidationMessageFor(Function(f) f.FileDocumentStatement, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row referencestatementdoc">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.PrevRefDocumentStatementName) : </label>
                    @Html.HiddenFor(Function(f) f.PrevRefDocumentStatementName)
                    <a id="linktostatementletter" href="" target="_blank">Letter of Statement</a>
                    <button id="DeleteDocStatement" name="DeleteButton" class="btn btn-sm" value="DeleteDocumentStatement"><i class="fa fa-close"></i></button>
                </div>
             </div>
             <div class="row transform">
                 <div class="form-group col-md-12">
                     <label>@Html.DisplayNameFor(Function(f) f.FileTransFormulir)</label>
                     <div class="input-group">
                         <input class="form-control" placeholder="File Transaction Form" id="FileTransFormulirName" />
                         @Html.CustomTextBoxFor(Function(f) f.FileTransFormulir, New With {.type = "file", .class = "hidden", .onchange = "ChangeText(this, 'FileTransFormulirName')"})
                         <span class="input-group-btn">
                             <a href="#" class="modal-link btn btn-default" id="btnclearuploadtransformulir" title="Clear"><i class='fa fa-refresh'></i></a>
                             <a href="" onclick="document.getElementById('FileTransFormulir').click(); return false" class="modal-link btn btn-default" id="btnsearchfiletransformulir" title="Browse"><i class='fa fa-search'></i></a>
                         </span>
                     </div>
                     @Html.ValidationMessageFor(Function(f) f.FileTransFormulir, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row referencetransformdoc">
                 <div class="form-group col-md-12">
                     <label>@Html.DisplayNameFor(Function(f) f.PrevRefTransactionFormName) : </label>
                     @Html.HiddenFor(Function(f) f.PrevRefTransactionFormName)
                     <a id="linktotransactionform" href="" target="_blank">Transaction Formulir</a>
                     <button id="DeleteTransactionForm" name="DeleteButton" class="btn btn-sm" value="DeleteTransactionForm"><i class="fa fa-close"></i></button>
                 </div>
             </div>
             <div class="row trandoc">
                 <div class="form-group col-md-6  ">
                     <label>@Html.DisplayNameFor(Function(f) f.NominalUnderlying)</label>
                     @Html.CustomTextBoxFor(Function(f) f.NominalUnderlying, New With {.class = "form-control autoNumeric", .placeholder = "Nominal Underlying"})
                     @Html.ValidationMessageFor(Function(f) f.NominalUnderlying, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.FileDocumentTransaction)</label>
                     <div class="input-group">
                         <input class="form-control" placeholder="File Document Transaction" id="FileDocumentTransactionName" />
                         @Html.CustomTextBoxFor(Function(f) f.FileDocumentTransaction, New With {.type = "file", .class = "hidden", .onchange = "ChangeText(this, 'FileDocumentTransactionName')"})
                         <span class="input-group-btn">
                             <a href="#" class="modal-link btn btn-default" id="btnclearuploaddoctransaction" title="Clear"><i class='fa fa-refresh'></i></a>
                             <a href="" onclick="document.getElementById('FileDocumentTransaction').click(); return false" class="modal-link btn btn-default" id="btnsearchfiledocstatement" title="Browse"><i class='fa fa-search'></i></a>
                         </span>
                     </div>
                     @Html.ValidationMessageFor(Function(f) f.FileDocumentTransaction, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row referencetrandoc">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.PrevRefDocumentTransactionName) : </label>
                     @Html.HiddenFor(Function(f) f.PrevRefDocumentTransactionName)
                     <a id="linktodocumenttransaction" href="" target="_blank">Document Transaction</a>
                     <button id="DeleteDocumentTrans" name="DeleteButton" class="btn btn-sm" value="DeleteDocumentTransaction"><i class="fa fa-close"></i></button>
                 </div>
                 <div class="form-group col-md-3">
                     <label>Nominal Underlying :</label>
                     <label class="lblNominalUnderlying"></label>
                 </div>
                 <div class="form-group col-md-3">
                     <label>Remaining Underlying :</label>
                     <label class="lblRemainingUnderlying"></label>
                 </div>
             </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                @If (Action = "Process") Then
                    @<button name="approvalButton" class="btn btn-primary" value="Approve"><i class="fa fa-check"></i> Approve</button>
                    @<button name="approvalButton" class="btn btn-primary" value="Reject"><i class="fa fa-close"></i> Reject</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Approval", "ExchangeTransaction")"><i class="fa fa-arrow-left"></i> Back</a>
                ElseIf (Action = "Deal") Or (Action = "Counter") Or (Action = "Edit") Then
                    @<button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExchangeTransaction")"><i class="fa fa-arrow-left"></i> Back</a>
                Else
                    @<button name="deleteButton" class="btn btn-primary" value="Delete"><i class="fa fa-trash"></i> Delete</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExchangeTransaction")"><i class="fa fa-arrow-left"></i> Back</a>
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
        @<div id="modal-container2" class="modal fade" tabindex="-1" role="dialog">
            <div class="modal-content">
            </div>
        </div>
    End Using
    @Section scripts
        <script>
            var action = '@(Action)';
            var CountListDeal = '@(CountListDeal)';
            if ('@Action' == 'Process' || '@Action' == 'Viewed') {
                $('#TransNum').attr('readonly', 'readonly');
                $('#DealNumber').attr('readonly', 'readonly');
                $('#AccNum').attr('readonly', 'readonly');
                $('#AccName').attr('readonly', 'readonly');
                TransactionType.disabled = true;
                RateType.disabled = true;
                TransactionCurrency.disabled = true;
                CustomerCurrency.disabled = true;
                $('#TransactionNominal').attr('readonly', 'readonly');
                $('#TransactionRate').attr('readonly', 'readonly');
                $('#TransactionNominal').attr('readonly', 'readonly');
                $('#CustomerNominal').attr('readonly', 'readonly');
                ValuePeriod.disabled = true;
                $('#ValueDate').attr('readonly', 'readonly');
                SourceFunds.disabled = true;
                $('#SourceAccNum').attr('readonly', 'readonly');
                $('#SourceAccName').attr('readonly', 'readonly');
                DocumentTransId.disabled = true;
                DocumentLHBUId.disabled = true;
                PurposeId.disabled = true;
                $('#DeleteDocumentTrans').addClass('hidden');
                $('#DeleteTransactionForm').addClass('hidden');
                $('#DeleteDocStatement').addClass('hidden');
            }
        </script>
        @Scripts.Render("~/Scripts/transaction.js")
    End Section
</div>
