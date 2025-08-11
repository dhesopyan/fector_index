var checkrate = true;
var firstview = true; // variable untuk mengecek awal kondisi load pertama (EDIT, VIEW atau CREATE)
var passlimitthreshold = false; //variabel untuk mengecek sudah lewat limit threshold atau belum
var passlimituser = false; // variabel untuk mengecek sudah lewat limit user atau belum
var countryCustomer = '';
var flaguploadunderlying = false; // variabel untuk mengecek perlu upload doc. underlying atau tidak
var showunderlying = false; //variabel untuk mengecek perlu memunculkan doc. underlying atau tidak
var tempdealnum = '';
var tid = '';
var trdocid = 0;
var tmpPurposeId = '';
var tmpDocLHBUId = '';

var dealcount = 0;
var totalbaseamount = 0.0;
var totalcounteramount = 0.0;

var lastamount = 0;

var flagReviewDoc = false;

var edited = false;

$(document).ready(function () {
    SetControl();

    var d = new Date();

    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + d.getFullYear();

    if (action == 'Deal'){
        $('#ValueDate').val(output);
    }

    $('#ValueDate').attr('readonly', 'readonly');
    $("#DocUnderlyingNominal").attr("readonly", "readonly");
    $('#DeleteDocStatementUnderlimit').addClass('hidden');
    $('#DeleteDocStatementOverlimit').addClass('hidden');
    //$('#DeleteTransactionForm').addClass('hidden');
    //$('#DeleteDocumentTrans').addClass('hidden');

    if (action == 'Edit' || action == 'Viewed' || action == 'Process' || action == 'ReviewDoc' || action == 'ReactiveTrans' || action == 'ViewDoc' || action == 'ViewHistory') {
        firstview = true;
        FirstLoadEdit();

        calculateConvertionWithGetRate($("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#CustomerCurrency").val(), $("#TransactionNominal").val());

        //checkDocumentRequirement();

        triggerMultiDeal();

        //var docunderlyingtype = $("#DocumentTransId").val();
        //var accno = $('#AccNum').val();
        //var docnum = $("#DocUnderlyingNum").val();
        //var docnominal = $("#DocUnderlyingNominal").val();
        //var transnominal = $("#TransactionNominal").val();
        //var transnum = $('#TransNum').val();

        //checkDocUnderlying(docunderlyingtype, accno, docnum, docnominal, transnominal, 'EDIT', transnum)

        if (action == 'Edit' || action == 'ReviewDoc') {
            $('#DeleteDocStatementUnderlimit').removeClass('hidden');
            $('#DeleteDocStatementOverlimit').removeClass('hidden');
            //$('#DeleteTransactionForm').removeClass('hidden');
            //$('#DeleteDocumentTrans').removeClass('hidden');

            if (action == 'Edit'){
                firstview = false;
            }

            if (action == 'ReviewDoc' || action == 'Reactive'){
                flagReviewDoc = true;
            }
        }
    }
    else {
        calculateConvertionWithGetRate($("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#CustomerCurrency").val(), $("#TransactionNominal").val());
        $('#DocUnderlyingNominal').val('0');
        firstview = false;
    }


    //CHECK IF WE 
    if (action == 'Edit' || action == 'Viewed' || action == 'Process' || action == 'ReviewDoc' || action == 'ReactiveTrans' || action == 'ViewDoc' || action == 'ViewHistory') {
        $("#AccNum, #TransactionType, #TransactionNominal, #SourceFunds, #SourceNominal, #SourceAccNum, #IsNetting"
        + ", #DerivativeType, #NettingType").change(function () {
            edited = true;
        });
    }
    else {
        edited = true;
    }
    
});

$('#BtnPrev').click(function () {
    if ($("#pnl2").is(":visible")) {
        $('#pnl1').removeClass('hidden');
        $('#pnl2').addClass('hidden');
        $('#pnl3').addClass('hidden');
        $('#pnl4').addClass('hidden');

        $('#BtnPrev').addClass('hidden');
        $('#BtnNext').removeClass('hidden');
    } else if ($("#pnl3").is(":visible")) {
        $('#pnl1').addClass('hidden');
        $('#pnl2').removeClass('hidden');
        $('#pnl3').addClass('hidden');
        $('#pnl4').addClass('hidden');

        $('#BtnPrev').removeClass('hidden');
        $('#BtnNext').removeClass('hidden');
    }
    else {
        $('#pnl1').addClass('hidden');
        $('#pnl2').addClass('hidden');
        $('#pnl3').removeClass('hidden');
        $('#pnl4').addClass('hidden');

        $('#BtnPrev').removeClass('hidden');
        $('#BtnNext').removeClass('hidden');

    }
    $('#pnlFooter').addClass('hidden');
});

$('#BtnNext').click(function () {
    if ($("#pnl1").is(":visible")) {
        if (firstview == false) {
            if ($('#AddAnotherTransaction').val() == 'false') {
                triggerMultiDeal();
            }
            if (action == 'Edit') {
                checkLimit($("#AccNum").val(), $("#AccName").val(), $("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#TransactionNominal").val(), $("#ValuePeriod").val(), 'EDIT', $("#TransNum").val());
            }
            else {
                checkLimit($("#AccNum").val(), $("#AccName").val(), $("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#TransactionNominal").val(), $("#ValuePeriod").val(), 'CREATE', '');
            }
        }
        $('#pnl1').addClass('hidden');
        $('#pnl2').removeClass('hidden');
        $('#pnl3').addClass('hidden');
        $('#pnl4').addClass('hidden');

        $('#BtnPrev').removeClass('hidden');
        $('#pnlFooter').addClass('hidden');
    } else if ($('#pnl2').is(":visible")) {
        if (firstview == false) {
            triggerMultiDeal();
            checkTransAmount($('#TransactionNominal').val(), totalbaseamount);

            if (action == 'Edit') {
                checkLimit($("#AccNum").val(), $("#AccName").val(), $("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#TransactionNominal").val(), $("#ValuePeriod").val(), 'EDIT', $("#TransNum").val());
            }
            else {
                checkLimit($("#AccNum").val(), $("#AccName").val(), $("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#TransactionNominal").val(), $("#ValuePeriod").val(), 'CREATE', '');
            }

            $('#pnl1').addClass('hidden');
            $('#pnl2').addClass('hidden');
            $('#pnl3').removeClass('hidden');
            $('#pnl4').addClass('hidden');

            $('#pnlFooter').addClass('hidden');
        }
        else {
            $('#pnl1').addClass('hidden');
            $('#pnl2').addClass('hidden');
            $('#pnl3').removeClass('hidden');
            $('#pnl4').addClass('hidden');

            $('#pnlFooter').addClass('hidden');
        }
    } else if ($('#pnl3').is(":visible")) {
        if (firstview == false || flagReviewDoc == true) {
            if ($("#SourceFunds").val() == 'Debit') {
                if ($("#SourceAccNum").val() == '' || $("#SourceAccName").val() == '') {
                    bootbox.alert("Please choose the source account number");
                    return;
                }
            }

            if ($("#SourceNominal").val() == '') {
                bootbox.alert("Please fill source nominal");
                return;
            }

            var isnetting = document.getElementById('IsNetting');
            if (isnetting.checked){
                if ($('#DerivativeType').val() == ''){
                    bootbox.alert("Please choose the derivative type");
                    return;
                }

                if ($('#NettingType').val() == '') {
                    bootbox.alert("Please choose the netting type");
                    return;
                }
            }

            checkDocStatement(action);
            checkDocumentRequirement();
            $('#pnl1').addClass('hidden');
            $('#pnl2').addClass('hidden');
            $('#pnl3').addClass('hidden');
            $('#pnl4').removeClass('hidden');

            $('#PassLimitThreshold').val(passlimitthreshold);
            $('#PassLimitUser').val(passlimituser);
            $('#FlagUploadUnderlying').val(flaguploadunderlying);

            $('#BtnNext').addClass('hidden');
            $('#pnlFooter').removeClass('hidden');
        }
        else {
            $('#pnl1').addClass('hidden');
            $('#pnl2').addClass('hidden');
            $('#pnl3').addClass('hidden');
            $('#pnl4').removeClass('hidden');

            $('#BtnNext').addClass('hidden');
            $('#pnlFooter').removeClass('hidden');
        }
    }
});

function SetControl() {
    $("#TransNum").attr("readonly", "readonly");
    $("#AccName").attr("readonly", "readonly");
    $("#AccNum").attr("readonly", "readonly");

    $(".useDeal").show();
    $(".docDetail").show();

    $("#TransactionType").attr('readonly', 'readonly');
    $("#RateType").prop("disabled", true);
    $("#TransactionCurrency").prop("disabled", true);
    $('#TransactionNominal').attr('readonly', 'readonly');
    $("#CustomerCurrency").prop("disabled", true);
    $('#TransactionRate').attr("readonly", "readonly");
    $('#ValuePeriod').prop("disabled", true);

    $("#SourceAccName").attr("readonly", "readonly");
    $("#SourceAccNum").attr("readonly", "readonly");
    $("#DocUnderlyingRemaining").attr("readonly", "readonly");
    $("#CustomerNominal").attr("readonly", "readonly");
    $("#FileDocumentTransactionName").attr("readonly", "readonly");
    $("#FileDocumentStatementUnderlimitName").attr("readonly", "readonly");
    $("#FileDocumentStatementOverlimitName").attr("readonly", "readonly");
    $("#FileTransFormulirName").attr("readonly", "readonly");

    if(action != 'Deal'){
        //if ($('#SourceFunds').val() == 'Cash') {
        //    $(".sof").hide();
        //}
        //else {
        //    $(".sof").show();
        //}
        $(".sof").show();
    }

    $('#pnl2').addClass('hidden');
    $('#pnl3').addClass('hidden');
    $('#pnl4').addClass('hidden');
    $('#BtnPrev').addClass('hidden');
    $('#pnlFooter').addClass('hidden');
}
String.prototype.replaceAll = function (str1, str2, ignore) {
    return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
}

function calculateConvertionWithGetRate(tranType, rateType, fromCurr, toCurr, amount) {
    if (firstview == false) {
        $.ajax({
            type: 'POST',
            url: 'JsOnGetRate',
            data: { tranType: tranType, rateType: rateType, fromCurr: fromCurr, toCurr: toCurr },
            cache: false,
            success: function (data) {
                newexrate = data.exchangeRate;
                $("#TransactionRate").val(parseFloat(newexrate).toLocaleString('en-US'));
                calculateConvertion(tranType, rateType, fromCurr, toCurr, amount, String(newexrate));
            }
        });
    }
}

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
    $("#CustomerNominal").val(parseFloat(convAmount).toLocaleString('en-US'));
    $("#SourceNominal").val(parseFloat(convAmount).toLocaleString('en-US'));
}

function SearchDeal(DealNumber, JoinStr) {
    $.ajax({
        type: 'GET',
        url: 'SearchDeal',
        data: { DealNumber: DealNumber, JoinStr: JoinStr },
        cache: false,
        success: function (result) {
            $('#DealNumber').val('');
            $('#tempDealNumber').val(result);
        }
    });
};

function triggerMultiDeal() {
    totalbaseamount = 0;
    totalcounteramount = 0;
    dealcount = 0;
    $('#TransDetail > tbody  > tr').each(function () {
        dealcount += 1;
        var curbaseamount = parseFloat($(this).find(".baseamount").val());
        var curcounteramount = parseFloat($(this).find(".counteramount").val());
        totalbaseamount = totalbaseamount + curbaseamount;
        totalcounteramount = totalcounteramount + curcounteramount;
    });

    if (dealcount > 1) {
        $("#TransactionNominal").attr("readonly", "readonly");
    }
    else {
        if(action != 'Viewed' && action != 'Process'){
            $("#TransactionNominal").removeAttr("readonly");
        }
        else {
            $("#TransactionNominal").attr("readonly");
        }
    }

    $("#TransactionNominal").val(parseFloat(totalbaseamount).toLocaleString("en-US"));
    $("#TransactionRate").val(parseFloat(totalcounteramount / totalbaseamount).toLocaleString('en-US'));
    $("#CustomerNominal").val(parseFloat(totalcounteramount).toLocaleString('en-US'));
    $("#SourceNominal").val(parseFloat(totalcounteramount).toLocaleString('en-US'));
    //checkDocumentRequirement();
}

function AddDoc(NewDocUnderlying, DocUnderlyingLink, TransFormLink) {
    trdocid = parseInt($('#tblDocDetail tr:last').attr('id').replace('doc', ''));

    trdocid = trdocid + 1;
    var rid = trdocid - 1;
    var tablerow = "";
    //var transnum = $('#TransNum').val();
    var doctransid = $('#DocumentTransId').val();
    var docunderlyingnum = $('#DocUnderlyingNum').val();
    var NominalDoc = $('#DocUnderlyingNominal').val();
    var doclhbuid = $('#DocumentLHBUId').val();
    if (doclhbuid == '') {
        doclhbuid = '998';
    }
    var purposeid = $('#PurposeId').val();

    var doctransactionlink = $('#FileDocumentTransactionName').val();
    var transformulirlink = $('#FileTransFormulirName').val();

    console.log(doctransactionlink);

    tablerow += "<tr id='doc" + trdocid + "'>";
    tablerow += "<td>";
    tablerow += trdocid;
    tablerow += "<input type='hidden' id='ListOfDoc.Index' name='ListOfDoc.Index' value='" + rid + "' />";
    tablerow += "<input type='hidden' class='id' id='ListOfDoc[" + rid + "].ID' name='ListOfDoc[" + rid + "].ID' value='" + trdocid + "' />";
    //tablerow += "<input type='hidden' id='ListOfDoc[" + rid + "].TransNum' name='ListOfDoc[" + rid + "].TransNum' value='" + transnum + "' />";
    tablerow += "</td>";
    tablerow += "<td>";
    tablerow += doctransid;
    tablerow += "<input type='hidden' id='ListOfDoc[" + rid + "].DocumentTransId' name='ListOfDoc[" + rid + "].DocumentTransId' value='" + doctransid + "' />";
    tablerow += "</td>";
    tablerow += "<td>";
    tablerow += docunderlyingnum;
    tablerow += "<input type='hidden' id='ListOfDoc[" + rid + "].DocUnderlyingNum' name='ListOfDoc[" + rid + "].DocUnderlyingNum' value='" + docunderlyingnum + "' />";
    tablerow += "</td>";
    tablerow += "<td>";
    tablerow += NominalDoc;
    tablerow += "<input type='hidden' id='ListOfDoc[" + rid + "].NominalDoc' name='ListOfDoc[" + rid + "].NominalDoc' value='" + NominalDoc + "' />";
    tablerow += "</td>";
    tablerow += "<td>";
    tablerow += doclhbuid;
    tablerow += "<input type='hidden' id='ListOfDoc[" + rid + "].DocumentLHBUId' name='ListOfDoc[" + rid + "].DocumentLHBUId' value='" + doclhbuid + "' />";
    tablerow += "</td>";
    tablerow += "<td>";
    tablerow += purposeid;
    tablerow += "<input type='hidden' id='ListOfDoc[" + rid + "].PurposeId' name='ListOfDoc[" + rid + "].PurposeId' value='" + purposeid + "' />";
    tablerow += "</td>";
    tablerow += "<td>";
    if (NewDocUnderlying){
        tablerow += doctransactionlink;
    }
    else {
        tablerow += "<a id='linktodocumenttransaction[" + rid + "]' href='" + DocUnderlyingLink + "' target='_blank'>Document Underlying - " + rid + "</a>";
    }
    tablerow += "<input type='hidden' id='ListOfDoc[" + rid + "].DocumentTransactionLink' name='ListOfDoc[" + rid + "].DocumentTransactionLink' value='" + doctransactionlink + "' />";
    tablerow += "</td>";
    tablerow += "<td>";
    //if (NewTransForm) {
        
    //}
    //else {
    //    tablerow += "<a id='linktotransformulir[" + rid + "]' href='" + TransFormLink + "' target='_blank'>Trans. Formulir - " + rid + "</a>";
    //}
    tablerow += transformulirlink;
    tablerow += "<input type='hidden' id='ListOfDoc[" + rid + "].TransFormulirLink' name='ListOfDoc[" + rid + "].TransFormulirLink' value='" + transformulirlink + "' />";
    tablerow += "</td>";
    tablerow += "<td>";
    tablerow += "<div><a href='#' title='Delete' id='DeleteDoc' data-rowid='doc" + trdocid + "'><span class='btn btn-info btn-sm fa fa-trash'></span><a></div>";
    tablerow += "</td>";
    tablerow += "</tr>";

    $('#tblDocDetail > tbody:last-child').append(tablerow);
}

$('body').on('click', '#BtnAddDoc', function (e) {
    e.preventDefault();
    $('#DocUnderlyingRemaining').val('0');
    var form = document.querySelector('form');
    var data = new FormData(form);

    $.ajax({
        type: 'POST',
        url: 'JsOnCheckDocSize',
        data: data,
        cache: false,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.FailUpload == false) {
                if (!$('.trandoc').is(":visible")) {
                    if (!$('.referencetrandoc').is(":visible")) {
                        AddDoc(true, '', '');
                    }
                    else {
                        AddDoc(false, document.getElementById("linktodocumenttransaction").href, '');
                    }
                }
                else {
                    AddDoc(true, '', '');
                }

                $('#DocUnderlyingRemaining').val('0');
                var form2 = document.querySelector('form');
                var data2 = new FormData(form2);

                $.ajax({
                    url: 'JsOnKeepDocDetail',
                    type: 'POST',
                    data: data2,
                    processData: false,
                    contentType: false
                });

                $('#DocumentTransId').select2().select2('val', '');
                $('#DocUnderlyingNum').val('')
                $('#DocUnderlyingNominal').val('0');
                $('#DocUnderlyingRemaining').val('0');
                $('#DocumentLHBUId').select2().select2('val', '');
                $('#PurposeId').select2().select2('val', '');
                $('#FileDocumentTransaction').val('');
                $('#FileDocumentTransactionName').val('');
                $('#FileTransFormulir').val('');
                $('#FileTransFormulirName').val('');
            }
            else {
                if (result.DocTransForm == true) {
                    bootbox.alert("Transaction Formulir file size is more than 2 MB, cannot upload");
                    return;
                }

                if (result.DocTrans == true) {
                    bootbox.alert("Document Transaction file size is more than 2 MB, cannot upload");
                    return;
                }
            }
        }
    });
});

$('body').on('click', '#btnadddeal', function (e) {
    e.preventDefault();
    if ($('#DealNumber').val() != '') {
        var dealnum = $('#DealNumber').val();

        tempdealnum = dealnum;

        $.ajax({
            url: 'JsOnCheckDealNumber',
            type: 'post',
            data: { dealnum: dealnum },
            dataType: 'json',
            success: function (data) {
                var dealfound = false;
                $('#TransDetail > tbody  > tr').each(function () {
                    if ($(this).find(".dealnumber").val() == dealnum) {
                        dealfound = true;
                    }
                });

                if (dealfound) {
                    bootbox.alert("Deal already registered");
                }
                else {
                    if (!data.errMessage) {
                        bootbox.dialog({
                            message: "Are your sure to add this deal: <br><br> Deal Number: " + data.dealNumber + "<br>Customer: " + data.accNumber + " - " + data.accName + "<br>Transaction Type: " + data.tranType + " - " + data.dealType + "<br>Currency: " + data.fromCurrency + " to " + data.toCurrency + ", Rate: " + data.strexchRate + "<br>Remaining Deal: " + data.strdealAmount + "<br>",
                            title: "Confirmation",
                            buttons: {
                                success: {
                                    label: "Yes",
                                    className: "btn-success",
                                    callback: function () {
                                        checkrate = false;

                                        $("#AccNum").val(data.accNumber);
                                        $("#AccName").val(data.accName);
                                        $('#TransactionType').select2().select2('val', data.tranType);
                                        $('#RateType').select2().select2('val', data.dealType);
                                        $('#TransactionCurrency').select2().select2('val', data.fromCurrency);
                                        $('#CustomerCurrency').select2().select2('val', data.toCurrency);
                                        $('#TransactionRate').val(parseFloat(data.exchRate).toLocaleString('en-US'));
                                        $('#ValuePeriod').select2().select2('val', data.valuePeriod);
                                        $('#ValueDate').val(data.strvalueDate);
                                        SetControl();

                                        tid = parseInt($('#TransDetail tr:last').attr('id'));

                                        tid = tid + 1;
                                        var rid = tid - 1;
                                        var tablerow = "";

                                        tablerow += "<tr id='" + tid + "'>";
                                        tablerow += "<td>";
                                        tablerow += data.dealNumber;
                                        tablerow += "<input type='hidden' id='ListOfDeal.Index' name='ListOfDeal.Index' value='" + rid + "' />";
                                        tablerow += "<input type='hidden' class='dealnumber' id='ListOfDeal[" + rid + "].DealNumber' name='ListOfDeal[" + rid + "].DealNumber' value='" + data.dealNumber + "' />";
                                        tablerow += "<input type='hidden' id='ListOfDeal[" + rid + "].BaseCurrency' name='ListOfDeal[" + rid + "].BaseCurrency' value='" + data.fromCurrency + "' />";
                                        tablerow += "<input type='hidden' id='ListOfDeal[" + rid + "].CounterCurrency' name='ListOfDeal[" + rid + "].CounterCurrency' value='" + data.toCurrency + "' />";
                                        tablerow += "</td>";
                                        tablerow += "<td>";
                                        tablerow += data.accNumber;
                                        tablerow += "<input type='hidden' id='ListOfDeal[" + rid + "].AccNumber' name='ListOfDeal[" + rid + "].AccNumber' value='" + data.accNumber + "' />";
                                        tablerow += "</td>";
                                        tablerow += "<td>";
                                        tablerow += data.accName;
                                        tablerow += "<input type='hidden' id='ListOfDeal[" + rid + "].AccName' name='ListOfDeal[" + rid + "].AccName' value='" + data.accName + "' />";
                                        tablerow += "</td>";
                                        tablerow += "<td>";
                                        tablerow += data.strexchRate;
                                        tablerow += "<input type='hidden' id='ListOfDeal[" + rid + "].TransRate' name='ListOfDeal[" + rid + "].TransRate' value='" + data.strexchRate + "' />";
                                        tablerow += "</td>";
                                        tablerow += "<td>";
                                        tablerow += data.strdealAmount;
                                        tablerow += "<input type='hidden' class='baseamount' id='ListOfDeal[" + rid + "].BaseNominal' name='ListOfDeal[" + rid + "].BaseNominal' value='" + data.dealAmount + "' />";
                                        tablerow += "</td>";
                                        tablerow += "<td>";
                                        tablerow += data.strcounterAmount;
                                        tablerow += "<input type='hidden' class='counteramount' id='ListOfDeal[" + rid + "].CounterNominal' name='ListOfDeal[" + rid + "].CounterNominal' value='" + data.counterAmount + "' />";
                                        tablerow += "</td>";
                                        tablerow += "<td>";
                                        tablerow += "<div><a href='#' title='Delete' id='DeleteDeal' data-rowid='" + tid + "'><span class='btn btn-info btn-sm fa fa-trash'></span><a></div>";
                                        tablerow += "</td>";
                                        tablerow += "</tr>";

                                        $('#TransDetail > tbody:last-child').append(tablerow);
                                        $('#DealNumber').val('');

                                        checkrate = true;

                                        triggerMultiDeal();

                                        fillDocUnderlyingType($("#AccNum").val());
                                    }
                                },
                                danger: {
                                    label: "No",
                                    className: "btn-danger",
                                    callback: function () {
                                        $('#DealNumber').val('');
                                    }
                                }
                            }
                        });
                    }
                    else {
                        bootbox.alert(data.errMessage);
                    }
                }
            },
        });
    }
});

function checkLimit(accno, accname, transtype, ratetype, basecurr, baseamount, dealperiod, mode, transnum) {
    if ($('#PurposeId').val() != "") {
        tmpPurposeId = $('#PurposeId').val();
    }
    if ($('#DocumentLHBUId').val() != "") {
        tmpDocLHBUId = $('#DocumentLHBUId').val();
    }
    if(firstview == false){
        $.ajax({
            type: 'POST',
            url: 'JsOnCheckLimit',
            data: { AccountNumber: accno, AccountName: accname, TransactionType: transtype, DealType: ratetype, BaseCurrency: basecurr, BaseAmount: baseamount, DealPeriod: dealperiod, Mode: mode, TransNum: transnum },
            cache: false,
            success: function (result) {
                countryCustomer = result.countryCustomer;
                //$('#DocumentTransId').select2().select2('val', '');

                if (result.UnderLimitThreshold) {
                    passlimitthreshold = false;

                    //fillAllLHBU();
                    if ($('#DocumentTransId').val() != "") {
                        getPurposeDropdown($('#DocumentTransId').val());
                    }
                    else {
                        fillAllPurpose();
                    }
                    $("#DocumentLHBUId").val('998');
                    $("#DocumentLHBUId").select2().select2('val', '998');
                    $("#PurposeId").val() == tmpPurposeId;
                    $("#PurposeId").select2().select2('val', tmpPurposeId);

                    DocumentLHBUId.disabled = true;
                }
                else {
                    passlimitthreshold = true;

                    if($('#DocumentTransId').val() != ""){
                        getLHBUDropdown($('#DocumentTransId').val());
                        getPurposeDropdown($('#DocumentTransId').val());
                    }
                    else {
                        fillAllLHBU();
                        fillAllPurpose();
                    }

                    $('#DocumentLHBUId').select2().select2('val', tmpDocLHBUId);
                    $("#PurposeId").select2().select2('val', tmpPurposeId);
                    DocumentLHBUId.disabled = false;

                    if(transtype == 'Sell'){
                        if (!result.HaveNPWP) {
                            bootbox.alert("Transaction limit reach and customer does not have NPWP. <br>Please complete Customer NPWP data in core banking!");
                            $('#BtnNext').addClass('hidden');
                            return;
                        } else {
                            $('#BtnNext').removeClass('hidden');
                        }
                    }
                    else {
                        $('#BtnNext').removeClass('hidden');
                    }
                    
                }

                if (result.UnderLimitUser) {
                    passlimituser = false;
                }
                else {
                    passlimituser = true;
                }
            }
        });
    }
    
};

function checkDocStatement(action) {
    if (firstview == false || flagReviewDoc == true) {
        $(".statementunderlimitdoc").hide();
        $(".referencestatementunderlimitdoc").hide();
        $(".statementoverlimitdoc").hide();
        $(".referencestatementoverlimitdoc").hide();
        $(".trandoc").hide();
        $(".referencetrandoc").hide();
        $(".transform").hide();
        $(".referencetransformdoc").hide();

        var accno = $("#AccNum").val();
        var convertedamount = parseFloat($("#TransactionNominal").val().replaceAll(',', '').replaceAll('.', ','));
        var basecurrency = $("#TransactionCurrency").val();
        var countercurrency = $("#CustomerCurrency").val();
        var trantype = $("#TransactionType").val();
        var ratetype = $("#RateType").val();
        var exchangerate = parseFloat($("#TransactionRate").val().replaceAll(',', '').replaceAll('.', ','));
        var transnum = $("#TransNum").val();
        var modeTransnum = action.concat("," + transnum);

        $.ajax({
            type: 'POST',
            url: 'JsOnCheckDocStatement',
            data: { accno: accno, convertedamount: convertedamount, basecurrency: basecurrency, trantype: trantype, ratetype: ratetype, modeTransnum: modeTransnum, edited: edited },
            cache: false,
            success: function (data) {
                if(!data.IsExcludeLHBU){
                    if (!data.IsOtherAccount) {
                        if (trantype == 'Sell') {
                            if (data.UnderLimit) {
                                //alert('underlimit');
                                $(".doctrans").hide();
                            }
                            else {
                                $(".doctrans").show();
                            }

                            if (data.UnderLimit && !data.HaveStatementUnderLimit) {
                                //alert('underlimit & not have statement underlimit');
                                //under limit and do not have statement letter this month
                                $(".statementunderlimitdoc").show();
                                $(".transform").show();
                            }
                            else if (data.UnderLimit && data.HaveStatementUnderLimit) {
                                //alert('underlimit & have statement underlimit');
                                //under limit and have previous statement letter this month
                                $(".referencestatementunderlimitdoc").show();
                                $(".transform").show();
                                $("#linktostatementunderlimitletter").attr("href", data.StatementUnderLimitName);
                                $("#PrevRefDocumentStatementUnderlimitName").val(data.StatementUnderLimitToSave);
                            }
                            else if (!data.UnderLimit && !data.HaveStatementOverLimit) {
                                //alert('overlimit & not have statement overlimit');
                                //over limit and doesn't have statement over limit this month
                                $(".statementoverlimitdoc").show();
                                $(".transform").show();
                            }
                            else if (!data.UnderLimit && data.HaveStatementOverLimit) {
                                //alert('overlimit & have statement overlimit');
                                //over limit and have previous statement letter
                                $(".referencestatementoverlimitdoc").show();
                                $(".transform").show();
                                $("#linktostatementoverlimitletter").attr("href", data.StatementOverLimitName);
                                $("#PrevRefDocumentStatementOverlimitName").val(data.StatementOverLimitToSave);
                            }
                        }
                        else {
                            $(".transform").show();
                        }
                    }
                }
                else {
                    $(".transform").show();
                }
            }
        })
    }
}

function checkDocumentRequirement() {
    if (firstview == false || flagReviewDoc == true) {
        $(".referencetrandoc").hide();
        $(".trandoc").hide();

        var accno = $("#AccNum").val();
        var convertedamount = parseFloat($("#TransactionNominal").val().replaceAll(',', '').replaceAll('.', ','));
        var trantype = $('#TransactionType').val();
        var docnum = $('#DocUnderlyingNum').val();

        $.ajax({
            type: 'POST',
            url: 'JsOnCheckDocumentRequirement',
            data: { accno: accno, convertedamount: convertedamount },
            cache: false,
            success: function (data) {
                if (!data.IsOtherAccount) {
                    if (trantype == 'Sell') {
                        if(docnum != ''){
                            if (data.PassNominalUnderlying || flaguploadunderlying) {
                                $(".trandoc").show();
                                $(".referencetrandoc").hide();
                            }
                            else {
                                if (data.DocTransLinkName != null && data.DocTransLinkSave != null){
                                    $(".trandoc").hide();
                                    $(".referencetrandoc").show();

                                    $("#PrevRefDocumentTransactionName").val(data.DocTransLinkName);
                                    $("#linktodocumenttransaction").attr("href", data.DocTransLinkSave);
                                }
                                else {
                                    $(".trandoc").show();
                                    $(".referencetrandoc").hide();

                                    $("#PrevRefDocumentTransactionName").val('');
                                    $("#linktodocumenttransaction").attr("href", '#');
                                }
                                
                            }
                        }
                        else {
                            if(data.PassNominalUnderlying){
                                $(".trandoc").show();
                                $(".referencetrandoc").hide();
                            }
                            else {
                                if(action == 'Edit'){
                                    $(".trandoc").hide();
                                    $(".referencetrandoc").show();

                                    if (data.DocTransLinkName != null && data.DocTransLinkSave != null) {
                                        $(".trandoc").hide();
                                        $(".referencetrandoc").show();

                                        $("#PrevRefDocumentTransactionName").val(data.DocTransLinkName);
                                        $("#linktodocumenttransaction").attr("href", data.DocTransLinkSave);
                                    }
                                    else {
                                        $(".trandoc").show();
                                        $(".referencetrandoc").hide();

                                        $("#PrevRefDocumentTransactionName").val('');
                                        $("#linktodocumenttransaction").attr("href", '#');
                                    }
                                }
                                else {
                                    $(".trandoc").show();
                                    $(".referencetrandoc").hide();
                                }
                            }
                        }
                    }
                }
            }
        });
    }
}

$('#SourceFunds').change(function () {
    //if ($('#SourceFunds').val() == 'Cash') {
    //    $(".sof").hide();
    //}
    //else {
    //    $(".sof").show();
    //}
    $(".sof").show();
});

function fillDocUnderlyingType(accno) {
    $.ajax({
        type: 'POST',
        url: 'JsOnFillDocUnderlyingType',
        data: { AccNumber: accno },
        dataType: 'json',
        success: function (data) {
            var markup = "<option value=''>Select Document Underlying</option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].DocumentId + ">" + data[x].Description + "</option>";
            }
            $("#DocumentTransId").html(markup).show();
            $("#DocumentTransId").select2({ width: '100%' });
        }
    });
};

$('#DocumentTransId').change(function () {
    var DocTrans = $(this).val();
    if (DocTrans != '') {
        getLHBUDropdown(DocTrans);
        getPurposeDropdown(DocTrans);
    }
});

function getLHBUDropdown(DocTrans) {
    $.ajax({
        url: 'JsOnGetLHBUDocument',
        type: 'post',
        dataType: 'json',
        data: { DocTrans: DocTrans },
        success: function (data) {
            if (passlimitthreshold) {
                var markup = "<option value=''>Select LHBU Document</option>";
                for (var x = 0; x < data.length; x++) {
                    markup += "<option value=" + data[x].DocumentId + ">" + data[x].Description + "</option>";
                }
                $("#DocumentLHBUId").html(markup).show();
                $("#DocumentLHBUId").select2({ width: '100%' });
            }
        }
    });
}

function getPurposeDropdown(DocTrans) {
    $.ajax({
        url: 'JsOnGetLHBUPurpose',
        type: 'post',
        dataType: 'json',
        data: { DocTrans: DocTrans },
        success: function (data) {
            var markup = "<option value=''>Select LHBU Purpose</option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].PurposeId + ">" + data[x].PurposeDisplay + "</option>";
            }
            $("#PurposeId").html(markup).show();
            $("#PurposeId").select2({ width: '100%' });
        }
    });
}

function checkDocUnderlying(docunderlyingtype, accnum, docnum, transnominal, mode, transnum) {
    $.ajax({
        type: 'POST',
        url: 'JsOnCheckDocUnderlying',
        dataType: 'json',
        data: { DocUnderlyingType: docunderlyingtype, AccNum: accnum, DocNum: docnum, TransNominal: transnominal, Mode: mode, Transnum: transnum },
        success: function (data) {
            
            if(!data.IsOwnerUnderlying){
                bootbox.alert("This document underlying is not belongs to this customer");
                return;
            }

            if (data.IsNewDoc) {
                flaguploadunderlying = true;
            }
            else {
                flaguploadunderlying = false;
            }

            if (data.DocNumFound) {
                $("#DocUnderlyingNominal").val(parseFloat(data.NominalUnderlying).toLocaleString('en-US'));
                $("#DocUnderlyingNominal").attr("readonly", "readonly");
                showunderlying = true;

                if (data.DocStatus == 'Estimation') {
                    bootbox.alert("The last document underlying is Estimation, please insert new document underlying after next");
                    flaguploadunderlying = true;
                    showunderlying = false;
                }
                else {
                    if (data.RemainingUnderlying < 0) {
                        bootbox.alert("Transaction nominal is greater than remaining document underlying nominal, please insert new document underlying after next");
                        flaguploadunderlying = true;
                        showunderlying = false;
                    }
                }
            }
            else {
                if ($('#DocUnderlyingNum').val() != '') {
                    $("#DocUnderlyingNominal").val(parseFloat(data.NominalUnderlying).toLocaleString('en-US'));
                    $("#DocUnderlyingNominal").removeAttr("readonly");
                    flaguploadunderlying = true;
                    showunderlying = false;
                }
                else {
                    $("#DocUnderlyingNominal").attr("readonly", "readonly");
                    flaguploadunderlying = false;
                    showunderlying = true;
                }
            }

            if (data.RemainingUnderlying != '0') {
                $("#DocUnderlyingRemaining").val(parseFloat(data.RemainingUnderlying).toLocaleString('en-US'));
            }
            else {
                if(action == 'Process' || action == 'Viewed'){
                    $("#DocUnderlyingRemaining").val('0');
                }
                else {
                    $("#DocUnderlyingRemaining").val('(NEW)');
                }
                
            }
        }
    });
}

$('#DocUnderlyingNum').change(function () {
    var docunderlyingtype = $("#DocumentTransId").val();
    var accno = $("#AccNum").val();
    var docnum = $("#DocUnderlyingNum").val();
    var docnominal = $("#DocUnderlyingNominal").val();
    var transnominal = $("#TransactionNominal").val();
    var transnum = $('#TransNum').val();

    if ($('#DocumentTransId').val() == '') {
        bootbox.alert("Please choose the document underlying type first");
    }
    else {
        if (docnominal == '') {
            docnominal = '0';
        }
        if (action == 'Deal') {
            checkDocUnderlying(docunderlyingtype, accno, docnum, transnominal, 'CREATE', 0)
        }
        else {
            checkDocUnderlying(docunderlyingtype, accno, docnum, transnominal, 'EDIT', transnum)
        }
    }
    if (action != 'ReviewDoc'){
        checkDocumentRequirement();
    }
});

function ChangeText(FileName, Target) {
    var FileInput = FileName.value.toString();
    var strFileName = FileInput;
    document.getElementById(Target).value = strFileName;
};

$("body").on("click", "#BtnSubmit", function (e) {
    e.preventDefault();

    trdocid = parseInt($('#tblDocDetail tr:last').attr('id').replace('doc', ''));

    if ($(".statementunderlimitdoc").is(":visible") && !$("#FileDocumentStatementUnderlimit").val()) {
        bootbox.alert("Please upload Letter of Statement(Underlimit)");
        return;
    }

    if ($(".statementoverlimitdoc").is(":visible") && !$("#FileDocumentStatementOverlimit").val()) {
        bootbox.alert("Please upload Letter of Statement(Overlimit");
        return;
    }

    if ($(".statementunderlimitdoc").is(":visible") && $(".statementoverlimitdoc").is(":visible")) {
        if (trdocid == 0) {
            if ($(".statementoverlimitdoc").is(":visible")) {
                bootbox.alert("Please upload file Transaction Formulir and Document Underlying");
                return;
            }
            else {
                bootbox.alert("Please upload file Transaction Formulir");
                return;
            }
        }
    }

    if ($('#SourceNominal').val() == 'NaN') {
        bootbox.alert('Source nominal is invalid');
        return;
    }

    if ($('#DocUnderlyingNominal').val() == 'NaN') {
        bootbox.alert('Doc. underlying nominal is invalid');
        return;
    }

    if ($('#DocUnderlyingRemaining').val() == 'NaN') {
        bootbox.alert('Doc. underlying remaining is invalid');
        return;
    }

    if ($('#TransactionRate').val() == 'NaN') {
        bootbox.alert('Base rate is invalid');
        return;
    }

    if ($('#CustomerNominal').val() == 'NaN') {
        bootbox.alert('Converted Amount is invalid');
        return;
    }

    if(trdocid == 0){
        if ($('#DocumentLHBUId').disabled == false) {
            if ($('#DocumentLHBUId').val() == '') {
                bootbox.alert("Please select document underlying type");
                return;
            }
        }

        if ($('#PurposeId').val() == '') {
            bootbox.alert("Please select purpose");
            return;
        }
    }
    
    //if ($(".transform").is(":visible") && !$("#FileTransFormulir").val()) {
    //    bootbox.alert("Please upload Transaction Form");
    //    return;
    //}

    //if ($(".trandoc").is(":visible") && !$("#FileDocumentTransaction").val()) {
    //    bootbox.alert("Please upload Transaction Document");
    //    return;
    //}

    if (tid == 1) {
        if ($('#TransactionNominal').val().replaceAll(',', '') != totalbaseamount) {
            bootbox.dialog({
                message: "Are you will add another transaction?",
                title: "Confirmation",
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success",
                        callback: function () {
                            $('#AddAnotherTransaction').val(true);
                            $("#TransactionNominal").removeAttr("readonly");

                            $("#TransactionType").removeAttr("readonly");
                            $("#RateType").removeAttr("disabled");
                            $("#TransactionCurrency").removeAttr("disabled");
                            $("#CustomerCurrency").removeAttr("disabled");
                            $('#ValuePeriod').removeAttr("disabled");
                            $('#DocumentLHBUId').removeAttr('readonly');

                            if ($('#DocUnderlyingRemaining').val() == '(NEW)') {
                                $('#DocUnderlyingRemaining').val('0');
                            }

                            $('#PassLimitThreshold').val(passlimitthreshold);
                            $('#PassLimitUser').val(passlimituser);
                            $('#FlagUploadUnderlying').val(flaguploadunderlying);

                            $("#frmData").submit();
                        }
                    },
                    danger: {
                        label: "No",
                        className: "btn-danger",
                        callback: function () {
                            if (totalbaseamount > $('#TransactionNominal').val()) {
                                bootbox.alert("The transaction value must be less than equal to amount deal at least");
                            }
                        }
                    }
                }
            });
        }
        else {
            $("#TransactionType").removeAttr("readonly");
            $("#RateType").removeAttr("disabled");
            $("#TransactionCurrency").removeAttr("disabled");
            $("#CustomerCurrency").removeAttr("disabled");
            $('#ValuePeriod').removeAttr("disabled");
            $('#DocumentLHBUId').removeAttr('readonly');

            if ($('#DocUnderlyingRemaining').val() == '(NEW)') {
                $('#DocUnderlyingRemaining').val('0');
            }

            $('#PassLimitThreshold').val(passlimitthreshold);
            $('#PassLimitUser').val(passlimituser);
            $('#FlagUploadUnderlying').val(flaguploadunderlying);

            $("#frmData").submit();
        }
    }
    else {
        $("#TransactionType").removeAttr("disabled");
        $("#RateType").removeAttr("disabled");
        $("#TransactionCurrency").removeAttr("disabled");
        $("#CustomerCurrency").removeAttr("disabled");
        $('#ValuePeriod').removeAttr("disabled");
        $('#DocumentLHBUId').removeAttr('readonly');

        if ($('#DocUnderlyingRemaining').val() == '(NEW)') {
            $('#DocUnderlyingRemaining').val('0');
        }

        $('#PassLimitThreshold').val(passlimitthreshold);
        $('#PassLimitUser').val(passlimituser);
        $('#FlagUploadUnderlying').val(flaguploadunderlying);

        $("#frmData").submit();
    }
});

$('body').on('click', '#btnsearchsourceaccnum', function (e) {
    e.preventDefault();
    var link = $(this).attr('href');
    $(this).attr('href', link);
    $(this).attr('data-target', '#modal-container2');
    $(this).attr('data-toggle', 'modal');
});

$("body").on("click", "#btnclearsourceaccnum", function (e) {
    e.preventDefault();
    $("#SourceAccName").val("");
    $("#SourceAccNum").val("")
});

$('body').on('click', '#btnclearuploaddocstatementunderlimit', function (e) {
    e.preventDefault();
    $('#FileDocumentStatementUnderlimitName').val("");
    $('#FileDocumentStatementUnderlimit').val("");
});

$('body').on('click', '#btnclearuploaddocstatementoverlimit', function (e) {
    e.preventDefault();
    $('#FileDocumentStatementOverlimitName').val("");
    $('#FileDocumentStatementOverlimit').val("");
});

$('body').on('click', '#btnclearuploaddoctransaction', function (e) {
    e.preventDefault();
    $('#FileDocumentTransactionName').val("");
    $('#FileDocumentTransaction').val("");
});

$('body').on('click', '#btnclearuploaddocstatement', function (e) {
    e.preventDefault();
    $('#FileDocumentStatement').val("");
    $('#txtUploadFileDocStatement').val("");
});
$('body').on('click', '#btnclearuploadtransformulir', function (e) {
    e.preventDefault();
    $('#FileTransFormulir').val("");
    $('#txtUploadFileTransFormulir').val("");
});

function DelDocument(FilePath, ButtonValue) {
    $.ajax({
        type: 'POST',
        url: 'DeleteDocument',
        data: { FilePath: FilePath, ButtonValue: ButtonValue },
        cache: false,
        success: function (result) {
            if (result.Deleted == true) {
                if (ButtonValue == 'DeleteDocumentStatementUnderlimit') {
                    $(".referencestatementunderlimitdoc").hide();
                    $(".statementunderlimitdoc").show();
                }
                if (ButtonValue == 'DeleteDocumentStatementOverlimit') {
                    $(".referencestatementoverlimitdoc").hide();
                    $(".statementoverlimitdoc").show();
                }
            }
            else {
                bootbox.alert("File not found in this directory");
            }

        }
    });
};

$('body').on('click', '#DeleteDocStatementUnderlimit', function (e) {
    e.preventDefault();
    bootbox.dialog({
        message: "Are your sure to delete this letter of statement?",
        title: "Confirmation",
        buttons: {
            success: {
                label: "Yes",
                className: "btn-success",
                callback: function () {
                    DelDocument($('#PrevRefDocumentStatementUnderlimitName').val(), $('#DeleteDocStatementUnderlimit').val());
                }
            },
            danger: {
                label: "No",
                className: "btn-danger"
            }
        }
    });
});

$('body').on('click', '#DeleteDocStatementOverlimit', function (e) {
    e.preventDefault();
    bootbox.dialog({
        message: "Are your sure to delete this letter of statement?",
        title: "Confirmation",
        buttons: {
            success: {
                label: "Yes",
                className: "btn-success",
                callback: function () {
                    DelDocument($('#PrevRefDocumentStatementOverlimitName').val(), $('#DeleteDocStatementOverlimit').val());
                }
            },
            danger: {
                label: "No",
                className: "btn-danger"
            }
        }
    });
});

$('body').on('click', '#DeleteDeal', function (e) {
    var rowid = $(this).data("rowid");
    e.preventDefault();
    bootbox.dialog({
        message: "Are your sure to delete this deal?",
        title: "Confirmation",
        buttons: {
            success: {
                label: "Yes",
                className: "btn-success",
                callback: function () {
                    $('#' + rowid).remove();
                    triggerMultiDeal();
                }
            },
            danger: {
                label: "No",
                className: "btn-danger"
            }
        }
    });
});

$('body').on('click', '#DeleteDoc', function (e) {
    var rowid = $(this).data("rowid");
    e.preventDefault();
    bootbox.dialog({
        message: "Are your sure to delete this document?",
        title: "Confirmation",
        buttons: {
            success: {
                label: "Yes",
                className: "btn-success",
                callback: function () {
                    $.ajax({
                        type: 'POST',
                        url: 'JsOnDeleteDoc',
                        data: { docid: rowid },
                        cache: false,
                        success: function (data) {
                            $('#' + rowid).remove();
                        }
                    });
                }
            },
            danger: {
                label: "No",
                className: "btn-danger"
            }
        }
    });
});

$("#TransactionNominal").change(function () {
    calculateConvertion($("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#CustomerCurrency").val(), $("#TransactionNominal").val(), $("#TransactionRate").val());
});

function FirstLoadEdit() {
    $(".statementunderlimitdoc").hide();
    $(".referencestatementunderlimitdoc").hide();
    $(".statementoverlimitdoc").hide();
    $(".referencestatementoverlimitdoc").hide();
    $(".referencetrandoc").hide();
    $(".referencetransformdoc").hide();
    $(".transform").hide();
    $(".trandoc").hide();

    var transnum = $("#TransNum").val()

    $.ajax({
        type: 'POST',
        url: 'JsOnFirstLoadEdit',
        data: { TransNum: transnum },
        cache: false,
        success: function (data) {
            if(!data.IsOtherAccount){
                if ($('#TransactionType').val() == 'Sell') {
                    if (data.UnderLimit) {
                        $(".doctrans").hide();
                    }
                    else {
                        $(".doctrans").show();
                    }

                    if (data.HaveDocTransLink) {
                        $(".referencetrandoc").show();
                        $("#linktodocumenttransaction").attr("href", data.DocTransLinkSave);
                        $("#PrevRefDocumentTransactionName").val(data.DocTransLinkName);
                    }

                    if (data.HaveDocStatementUnderlimitLink) {
                        $(".referencestatementunderlimitdoc").show();
                        $("#linktostatementunderlimitletter").attr("href", data.DocStatementUnderlimitLinkSave);
                        $("#PrevRefDocumentStatementUnderlimitName").val(data.DocStatementUnderlimitLinkName);
                    }

                    if (data.HaveDocStatementOverlimitLink) {
                        $(".referencestatementoverlimitdoc").show();
                        $("#linktostatementoverlimitletter").attr("href", data.DocStatementOverlimitLinkSave);
                        $("#PrevRefDocumentStatementOverlimitName").val(data.DocStatementOverlimitLinkName);
                    }

                    if (data.HaveTransFormulirLink) {
                        $(".referencetransformdoc").show();
                        $("#linktotransactionform").attr("href", data.TransFormulirLinkSave);
                        $("#PrevRefTransactionFormName").val(data.TransFormulirLinkName);
                    }
                }
                else {
                    if (data.HaveTransFormulirLink) {
                        $(".referencetransformdoc").show();
                        $("#linktotransactionform").attr("href", data.TransFormulirLinkSave);
                        $("#PrevRefTransactionFormName").val(data.TransFormulirLinkName);
                    }
                }
            }
        }
    });
}

function checkTransAmount(amount, totalBaseamount) {
    $.ajax({
        type: 'POST',
        url: 'JsOnCheckTransAmount',
        data: { Amount: amount, TotalBaseAmount: totalBaseamount },
        cache: false,
        success: function (data) {
            if (data.MoreThanBaseAmount == true) {
                bootbox.alert("The transaction value is more than total amount deal");
                return;
            }
        }
    });
}

function checkNominalUnderlying(amount, remainNominalUnderlying) {
    $.ajax({
        type: 'POST',
        url: 'JsOnCheckNominalUnderlying',
        data: { Amount: amount, RemainingNominalUnderlying: remainNominalUnderlying },
        cache: false,
        success: function (data) {
            if (data.MoreThanRemainNominalUnderlying) {
                bootbox.alert("The transaction value is more than remaining underlying, please insert new document underlying");
                return;
            }
        }
    });
}

function fillAllLHBU() {
    $.ajax({
        type: 'POST',
        url: 'JsOnFillAllLHBU',
        dataType: 'json',
        success: function (data) {
            var markup = "<option value=''>Select LHBU Document</option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].DocumentId + ">" + data[x].DocumentLHBUDisplay + "</option>";
            }
            $("#DocumentLHBUId").html(markup).show();
            $("#DocumentLHBUId").select2({ width: '100%' });
        }
    });
};

function fillAllPurpose() {
    $.ajax({
        type: 'POST',
        url: 'JsOnFillAllPurpose',
        dataType: 'json',
        success: function (data) {
            var markup = "<option value=''>Select LHBU Purpose</option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].PurposeId + ">" + data[x].PurposeDisplay + "</option>";
            }
            $("#PurposeId").html(markup).show();
            $("#PurposeId").select2({ width: '100%' });
        }
    });
};