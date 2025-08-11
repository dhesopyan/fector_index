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
    Dim DerivativeType As IEnumerable(Of SelectListItem) = ViewBag.DerivativeType
    Dim NettingType As IEnumerable(Of SelectListItem) = ViewBag.NettingType
    Dim CountListDeal As Integer = 0
    Dim CountListDoc As Integer = 0
    Dim FlagAddAnotherTrans As string = ViewBag.AddAnotherTransaction
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
        @<div class="panel-body" id="pnl1">
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
                                            @Html.HiddenFor(Function(f) f.ListOfDeal(CInt(id)).DealNumber, New With {.class = "dealnumber"})
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
                                                @If Action = "Process" Or Action = "Viewed" Or Action = "ViewDoc" Then
                                                    @<a href='#' title='Delete' id='DeleteDeal' name="DeleteDeal" data-rowid='@CInt(rowid)' class="hidden"><span class='btn btn-info btn-sm fa fa-trash'></span></a>
                                                Else
                                                    @<a href='#' title='Delete' id='DeleteDeal' name="DeleteDeal" data-rowid='@CInt(rowid)'><span class='btn btn-info btn-sm fa fa-trash'></span></a>
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
                    @Html.CustomTextBoxFor(Function(f) f.AccNum, New With {.class = "form-control", .placeholder = "Account Number"})
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
                    <label>@Html.DisplayNameFor(Function(f) f.RateType)</label>
                    @Html.DropDownListFor(Function(f) f.RateType, RateTypeOption, New With {.class = "form-control", .placeholder = "Rate Type"})
                    @Html.ValidationMessageFor(Function(f) f.RateType, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-body" id="pnl2">
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
                 <div class="form-group col-md-6">
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
        </div>
        @<div class="panel-body" id="pnl3">
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.SourceFunds)</label>
                    @Html.DropDownListFor(Function(f) f.SourceFunds, SourceFundsOption, "Select Source Funds", New With {.class = "form-control", .placeholder = "Source of Funds"})
                    @Html.ValidationMessageFor(Function(f) f.SourceFunds, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6  ">
                    <label>@Html.DisplayNameFor(Function(f) f.SourceNominal)</label>
                    @Html.CustomTextBoxFor(Function(f) f.SourceNominal, New With {.class = "form-control autoNumeric", .placeholder = "Source Nominal"})
                    @Html.ValidationMessageFor(Function(f) f.SourceNominal, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row sof">
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
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.IsNetting)</label>
                    <br />
                   @Html.CheckBoxFor(Function(f) f.IsNetting) @Html.DisplayNameFor(Function(f) f.IsNetting)
                </div>
                <div class="form-group col-md-6  ">
                    <label>@Html.DisplayNameFor(Function(f) f.DerivativeType)</label>
                    @Html.DropDownListFor(Function(f) f.DerivativeType, DerivativeType, "Select Derivative Type", New With {.class = "form-control", .placeholder = "Derivative Type"})
                    @Html.ValidationMessageFor(Function(f) f.DerivativeType, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6  ">
                    <label>@Html.DisplayNameFor(Function(f) f.NettingType)</label>
                    @Html.DropDownListFor(Function(f) f.NettingType, NettingType, "Select Netting Type", New With {.class = "form-control", .placeholder = "Netting Type"})
                    @Html.ValidationMessageFor(Function(f) f.NettingType, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-body" id="pnl4">
            <div class="row statementunderlimitdoc">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.FileDocumentStatementUnderlimit)</label>
                    <div class="input-group">
                        <input class="form-control" placeholder="File Letter of Statement(Underlimit)" id="FileDocumentStatementUnderlimitName" />
                        @Html.CustomTextBoxFor(Function(f) f.FileDocumentStatementUnderlimit, New With {.type = "file", .class = "hidden", .onchange = "ChangeText(this, 'FileDocumentStatementUnderlimitName')"})
                        <span class="input-group-btn">
                            <a href="#" class="modal-link btn btn-default" id="btnclearuploaddocstatementunderlimit" title="Clear"><i class='fa fa-refresh'></i></a>
                            <a href="" onclick="document.getElementById('FileDocumentStatementUnderlimit').click(); return false" class="modal-link btn btn-default" id="btnsearchfiledocstatementunderlimit" title="Browse"><i class='fa fa-search'></i></a>
                        </span>
                    </div>
                    @Html.ValidationMessageFor(Function(f) f.FileDocumentStatementUnderlimit, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row referencestatementunderlimitdoc">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.PrevRefDocumentStatementUnderlimitName) : </label>
                    @Html.HiddenFor(Function(f) f.PrevRefDocumentStatementUnderlimitName)
                    <a id="linktostatementunderlimitletter" href="" onclick="window.open(this.href, 'mywin',
'left=290,top=40,width=500,height=500,toolbar=1,resizable=0'); return false;">Letter of Statement(Underlimit)</a>
                    <button id="DeleteDocStatementUnderlimit" name="DeleteButton" class="btn btn-sm btn btn-primary" value="DeleteDocumentStatementUnderlimit"><i class="fa fa-trash"></i></button>
                </div>
            </div>
            <div class="row statementoverlimitdoc">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.FileDocumentStatementOverlimit)</label>
                    <div class="input-group">
                        <input class="form-control" placeholder="File Letter of Statement(Overlimit)" id="FileDocumentStatementOverlimitName" />
                        @Html.CustomTextBoxFor(Function(f) f.FileDocumentStatementOverlimit, New With {.type = "file", .class = "hidden", .onchange = "ChangeText(this, 'FileDocumentStatementOverlimitName')"})
                        <span class="input-group-btn">
                            <a href="#" class="modal-link btn btn-default" id="btnclearuploaddocstatementoverlimit" title="Clear"><i class='fa fa-refresh'></i></a>
                            <a href="" onclick="document.getElementById('FileDocumentStatementOverlimit').click(); return false" class="modal-link btn btn-default" id="btnsearchfiledocstatementoverlimit" title="Browse"><i class='fa fa-search'></i></a>
                        </span>
                    </div>
                    @Html.ValidationMessageFor(Function(f) f.FileDocumentStatementOverlimit, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row referencestatementoverlimitdoc">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.PrevRefDocumentStatementOverlimitName) : </label>
                    @Html.HiddenFor(Function(f) f.PrevRefDocumentStatementOverlimitName)
                    <a id="linktostatementoverlimitletter" href="" onclick="window.open(this.href, 'mywin',
'left=290,top=40,width=500,height=500,toolbar=1,resizable=0'); return false;">Letter of Statement(Overlimit)</a>
                    <button id="DeleteDocStatementOverlimit" name="DeleteButton" class="btn btn-sm btn btn-primary" value="DeleteDocumentStatementOverlimit"><i class="fa fa-trash"></i></button>
                </div>
            </div>
            <div class="row docunderlyingtype">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DocumentTransId)</label>
                    @Html.DropDownListFor(Function(f) f.DocumentTransId, DocumentTransOption, "Select Document Underlying Type", New With {.class = "form-control", .placeholder = "Document Transaction"})
                    @Html.ValidationMessageFor(Function(f) f.DocumentTransId, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DocUnderlyingNum)</label>
                    @Html.CustomTextBoxFor(Function(f) f.DocUnderlyingNum, New With {.class = "form-control", .placeholder = "Document Underlying Number"})
                    @Html.ValidationMessageFor(Function(f) f.DocUnderlyingNum, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DocUnderlyingNominal)</label>
                    @Html.CustomTextBoxFor(Function(f) f.DocUnderlyingNominal, New With {.class = "form-control autoNumeric", .placeholder = "Document Underlying Nominal"})
                    @Html.ValidationMessageFor(Function(f) f.DocUnderlyingNominal, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DocUnderlyingRemaining)</label>
                    @Html.CustomTextBoxFor(Function(f) f.DocUnderlyingRemaining, New With {.class = "form-control autoNumeric", .placeholder = "Document Underlying Remaining"})
                    @Html.ValidationMessageFor(Function(f) f.DocUnderlyingRemaining, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6 doclhbu">
                    <label>@Html.DisplayNameFor(Function(f) f.DocumentLHBUId)</label>
                    @Html.DropDownListFor(Function(f) f.DocumentLHBUId, LHBUDocumentOption, "Select LHBU Document", New With {.class = "form-control", .placeholder = "LHBU Document"})
                    @Html.ValidationMessageFor(Function(f) f.DocumentLHBUId, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6 docpurpose">
                    <label>@Html.DisplayNameFor(Function(f) f.PurposeId)</label>
                    @Html.DropDownListFor(Function(f) f.PurposeId, PurposeIdOption, "Select LHBU Purpose", New With {.class = "form-control", .placeholder = "LHBU Purpose"})
                    @Html.ValidationMessageFor(Function(f) f.PurposeId, String.Empty, New With {.class = "help-block"})
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
                    <a id="linktotransactionform" href="" onclick="window.open(this.href, 'mywin',
'left=290,top=40,width=500,height=500,toolbar=1,resizable=0'); return false;">Transaction Formulir</a>
                    <button id="DeleteTransactionForm" name="DeleteButton" class="btn btn-sm btn btn-primary" value="DeleteTransactionForm"><i class="fa fa-trash"></i></button>
                </div>
            </div>
            <div class="row trandoc">
                <div class="form-group col-md-12">
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
                    <a id="linktodocumenttransaction" href="" onclick="window.open(this.href, 'mywin',
'left=290,top=40,width=500,height=500,toolbar=1,resizable=0'); return false;">Document Transaction</a>
                    <button id="DeleteDocumentTrans" name="DeleteButton" class="btn btn-sm btn btn-primary" value="DeleteDocumentTransaction"><i class="fa fa-trash"></i></button>
                </div>
            </div>
            <div class="form-group pull-right" id="divAddDoc">
                @If (Action = "Deal") Or (Action = "Edit") Or (Action = "ReviewDoc") Then
                    @<a id="BtnAddDoc" class="btn btn-primary" href="#"><i class="fa fa-plus"></i> Add</a>
                Else
                    @<a id="BtnAddDoc" class="btn btn-primary hidden" href="#"><i class="fa fa-plus"></i> Add</a>
                End If
                <div class="clearfix"></div>
            </div>
            <div class="row docDetail" id="temp2">
                <div class="form-group col-md-12">
                    <table id="tblDocDetail" class="table FectorTable">
                        <thead>
                            <tr id="doc0" class="GvHeader">
                                <th width="7%">
                                    No.
                                </th>
                                <th width="15%">
                                    Doc. Underlying
                                </th>
                                <th width="15%">
                                    Doc. Number
                                </th>
                                <th width="15%">
                                    Doc. Nominal
                                </th>
                                <th width="10%">
                                    Doc. LHBU
                                </th>
                                <th width="15%">
                                    Purpose
                                </th>
                                <th width="15%">
                                    Doc. Underlying Link
                                </th>
                                <th width="15%">
                                    Trans. Formulir Link
                                </th>
                                <th width="10%">&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                            @If Not IsNothing(Model.ListOfDoc) Then
                            CountListDoc = 1
                            Dim rowid As Integer = 1
                            Dim id As Integer = rowid - 1
                                @For Each dtl As Fector_Index.ExchangeTransactionDoc In Model.ListOfDoc
                                Dim trid As String = "doc" & rowid
                                Dim docid As String = "doc" & id
                                    @<tr id="@trid">
                                        <td>
                                            @dtl.ID 
                                            <input type='hidden' id='ListOfDoc.Index' name='ListOfDoc.Index' value='@CInt(id)' />
                                            @Html.HiddenFor(Function(f) f.ListOfDoc(CInt(id)).ID, New With {.class = "id"})
                                            @Html.HiddenFor(Function(f) f.ListOfDoc(CInt(id)).TransNum)
                                        </td>
                                        <td>
                                            @dtl.DocumentTransId 
                                            @Html.HiddenFor(Function(f) f.ListOfDoc(CInt(id)).DocumentTransId, New With {.class = "doctransid"})
                                        </td>
                                        <td>
                                            @dtl.DocUnderlyingNum 
                                            @Html.HiddenFor(Function(f) f.ListOfDoc(CInt(id)).DocUnderlyingNum)
                                        </td>
                                        <td>
                                            @dtl.NominalDoc 
                                            @Html.HiddenFor(Function(f) f.ListOfDoc(CInt(id)).NominalDoc)
                                        </td>
                                        <td>
                                            @dtl.DocumentLHBUId 
                                            @Html.HiddenFor(Function(f) f.ListOfDoc(CInt(id)).DocumentLHBUId)
                                        </td>
                                        <td>
                                            @dtl.PurposeId 
                                            @Html.HiddenFor(Function(f) f.ListOfDoc(CInt(id)).PurposeId)
                                        </td>
                                        <td>
                                            @*@dtl.DocumentTransactionLink*@ 
                                            @If dtl.DocumentTransactionLink <> "" Then
                                                @<a id="linktodocumenttransaction" href="@UrlHelper.GenerateContentUrl(dtl.DocumentTransactionLink, Me.Context)" onclick="window.open(this.href, 'mywin',
'left=290,top=40,width=500,height=500,toolbar=1,resizable=0'); return false;">Document Transaction - @CInt(id)</a>
                                            End If
                                            @Html.HiddenFor(Function(f) f.ListOfDoc(CInt(id)).DocumentTransactionLink)
                                        </td>
                                        <td>
                                            @If dtl.TransFormulirLink <> "" Then
                                                @<a id="linktotransform" href="@UrlHelper.GenerateContentUrl(dtl.TransFormulirLink, Me.Context)" onclick="window.open(this.href, 'mywin',
'left=290,top=40,width=500,height=500,toolbar=1,resizable=0'); return false;">Trans. Form -@CInt(id)</a>
                End If
                                            @Html.HiddenFor(Function(f) f.ListOfDoc(CInt(id)).TransFormulirLink)
                                        </td>
                                        <td>
                                            <div id="divDeleteDoc">
                                                @If Action = "Process" Or Action = "Viewed" Or Action = "ReactiveTrans" or Action = "ViewDoc" Then
                                                    @<a href='#' title='Delete' id='DeleteDoc' name="DeleteDoc" data-rowid='@trid' class="hidden"><span class='btn btn-info btn-sm fa fa-trash'></span></a>
                                                Else
                                                    @<a href='#' title='Delete' id='DeleteDoc' name="DeleteDoc" data-rowid='@trid'><span class='btn btn-info btn-sm fa fa-trash'></span></a>
                                                End If
                                            </div>
                                        </td>
                                    </tr>
                                rowid += 1
                                id = rowid - 1
                                Next
                                Else
                            CountListDoc = 0
                            End If
                        </tbody>
                    </table>
                </div>
            </div>
             @Html.CustomTextBoxFor(Function(f) f.PassLimitThreshold, New With {.class = "form-control hidden"})
             @Html.CustomTextBoxFor(Function(f) f.PassLimitUser, New With {.class = "form-control hidden"})
             @Html.CustomTextBoxFor(Function(f) f.FlagUploadUnderlying, New With {.class = "form-control hidden"})
             @Html.CustomTextBoxFor(Function(f) f.AddAnotherTransaction, New With {.class = "form-control hidden"})
        </div>
        @<div class="panel-body">
            <div class="pull-right">
                <a id="BtnPrev" class="btn btn-danger" href="#"><i class="fa fa-arrow-left"></i> Prev</a>
                <a id="BtnNext" class="btn btn-danger" href="#"><i class="fa fa-arrow-right"></i> Next</a>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-footer" id="pnlFooter">
            <div class="pull-right">
                @If (Action = "Process") Then
                    @<button name="approvalButton" class="btn btn-primary" value="Approve"><i class="fa fa-check"></i> Approve</button>
                    @<button name="approvalButton" class="btn btn-primary" value="Reject"><i class="fa fa-close"></i> Reject</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Approval", "ExchangeTransaction")"><i class="fa fa-arrow-left"></i> Back</a>
                ElseIf (Action = "Deal") Or (Action = "Counter") Or (Action = "Edit") Then
                    @<button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExchangeTransaction")"><i class="fa fa-arrow-left"></i> Back</a>
                ElseIf Action = "Viewed" Then
                    @<button name="deleteButton" class="btn btn-primary" value="Delete"><i class="fa fa-trash"></i> Delete</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExchangeTransaction")"><i class="fa fa-arrow-left"></i> Back</a>
                ElseIf Action = "ViewHistory" Then
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExchangeTransactionHistory")"><i class="fa fa-arrow-left"></i> Back</a>
                ElseIf Action = "ReviewDoc" Then
                    @<button name="Review" class="btn btn-primary" value="Review"><i class="fa fa-check"></i> Review</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExchangeTransactionReview")"><i class="fa fa-arrow-left"></i> Back</a>
                ElseIf Action = "ReactiveTrans" Then
                    @<button name="ReactiveTrans" class="btn btn-primary" value="ReactiveTrans"><i class="fa fa-check"></i> Reactive</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExchangeTransactionReactive")"><i class="fa fa-arrow-left"></i> Back</a>
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
            var CountListDoc = '@(CountListDoc)';
            var FlagAddAnother = '@(FlagAddAnotherTrans)';

            if (action != 'Viewed' && action != 'Process' && action != 'ViewHistory' && action != 'ReviewDoc' && action != 'ReactiveTrans' && action != 'ViewDoc') {
                if ('@FlagAddAnotherTrans' == 'yes') {
                    $("#DealNumber").attr('readonly', 'readonly');
                    $('#btnadddeal').attr('disabled', 'disabled');
                    $('#DeleteDeal').addClass('hidden');

                    $('#TransactionNominal').removeAttr('readonly');
                }
                else {
                    $('#DealNumber').removeAttr('readonly');
                    $('#btnadddeal').removeAttr('disabled');
                    $('#DeleteDeal').removeClass('hidden');
                }
            }
            else {
                $("#DealNumber").attr('readonly', 'readonly');
                $('#btnadddeal').attr('disabled', 'disabled');
                $('#DeleteDeal').addClass('hidden');
            }
            

            if ('@Action' == 'Process' || '@Action' == 'Viewed' || '@Action' == 'ViewHistory' || '@Action' == 'ReviewDoc' || '@Action' == 'ReactiveTrans' || '@Action' == 'ViewDoc') {
                $('#TransNum').attr('readonly', 'readonly');
                $('#DealNumber').attr('readonly', 'readonly');
                $('#AccNum').attr('readonly', 'readonly');
                $('#AccName').attr('readonly', 'readonly');
                $('#TransactionType').attr('readonly', 'readonly');
                RateType.disabled = true;
                TransactionCurrency.disabled = true;
                CustomerCurrency.disabled = true;
                $('#TransactionNominal').attr('readonly', 'readonly');
                $('#TransactionRate').attr('readonly', 'readonly');
                $('#CustomerNominal').attr('readonly', 'readonly');
                ValuePeriod.disabled = true;
                $('#ValueDate').attr('readonly', 'readonly');
                SourceFunds.disabled = true;
                $('#SourceAccNum').attr('readonly', 'readonly');
                $('#SourceAccName').attr('readonly', 'readonly');
                $('#SourceNominal').attr('readonly', 'readonly');
                IsNetting.disabled = true;
                DerivativeType.disabled = true;
                NettingType.disabled = true;
                if(action != 'ReviewDoc'){
                    $('#DocUnderlyingNum').attr('readonly', 'readonly');
                    DocumentTransId.disabled = true;
                    DocumentLHBUId.disabled = true;
                    PurposeId.disabled = true;
                }
                //$('#DeleteDocumentTrans').addClass('hidden');
                //$('#DeleteTransactionForm').addClass('hidden');
                if (action != 'ReviewDoc'){
                    $('#DeleteDocStatement').addClass('hidden');
                }
            }
        </script>
        @Scripts.Render("~/Scripts/transaction.js")
    End Section
</div>
