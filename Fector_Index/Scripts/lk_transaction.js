var checkrate = true;
var firstview = true;

$(document).ready(function () {
    $("#TransNum").attr("readonly", "readonly");
    $("#AccName").attr("readonly", "readonly");
    $("#AccNum").attr("readonly", "readonly");
    $("#SourceAccName").attr("readonly", "readonly");
    $("#SourceAccNum").attr("readonly", "readonly");
    $("#CustomerNominal").attr("readonly", "readonly");
    $("#FileDocumentTransactionName").attr("readonly", "readonly");
    $("#FileDocumentStatementName").attr("readonly", "readonly");
    $("#FileTransFormulirName").attr("readonly", "readonly");
        
    var d = new Date();

    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + d.getFullYear();

    $('#ValueDate').val(output);
    $('#ValueDate').attr('readonly', 'readonly');

    if (action == "Counter"){
        SetControl(false);
    }
    else if (action == "Deal") {
        SetControl(true);
    }
    else {
        if (CountListDeal == "0") {
            SetControl(false);
        }
        else {
            SetControl(true);
        }
    }
       
    if (action == 'Edit' || action == 'Viewed' || action == 'Process') {
        firstview = true;
        FirstLoadEdit();
      
        calculateConvertionWithGetRate($("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#CustomerCurrency").val(), $("#TransactionNominal").val());

        checkDocumentRequirement();

        triggerMultiDeal();
        if (action == 'Edit') {
            firstview = false;
        }
    }
    else {
        firstview = false;

        calculateConvertionWithGetRate($("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#CustomerCurrency").val(), $("#TransactionNominal").val());

        checkDocumentRequirement();

        triggerMultiDeal();
    }

    
});

function triggerMultiDeal()
{
    var dealcount = 0;
    var totalbaseamount = 0.0;
    var totalcounteramount = 0.0;
    $('#TransDetail > tbody  > tr').each(function () {
        dealcount += 1;
        var curbaseamount = parseFloat($(this).find(".baseamount").val());
        var curcounteramount = parseFloat($(this).find(".counteramount").val());
        totalbaseamount = totalbaseamount + curbaseamount;
        totalcounteramount = totalcounteramount + curcounteramount;
    });
    
    if (dealcount >= 1){
        $("#TransactionNominal").attr("readonly", "readonly");
        $("#TransactionNominal").val(parseFloat(totalbaseamount).toLocaleString('en-US'));
        $("#TransactionRate").val(parseFloat(totalcounteramount / totalbaseamount).toLocaleString('en-US'));
        $("#CustomerNominal").val(parseFloat(totalcounteramount).toLocaleString('en-US'));
    }
    else {
        $("#TransactionNominal").removeAttr("readonly");
        if (action != 'Edit' && action != 'Viewed' && action != 'Process') {
            $("#TransactionNominal").val("0.0");
            $("#CustomerNominal").val("0.0");
        }
    }
    checkDocumentRequirement();
}

function SetControl(useDeal) {
    if (useDeal) {
        if ($("#AccNum").val() != "") {
            $("#btnclearaccnum").prop("disabled", true);
            $("#btnclearaccnum").addClass("disabled");
            $("#btnclearsourceaccnum").prop("disabled", true);
            $("#btnclearsourceaccnum").addClass("disabled");
            $("#btnsearchaccnum").prop("disabled", true);
            $("#btnsearchaccnum").addClass("disabled");
            $("#btnsearchsourceaccnum").prop("disabled", true);
            $("#btnsearchsourceaccnum").addClass("disabled");
        }
        else {
            $("#btnclearaccnum").prop("disabled", false);
            $("#btnclearaccnum").removeClass("disabled");
            $("#btnclearsourceaccnum").prop("disabled", false);
            $("#btnclearsourceaccnum").removeClass("disabled");
            $("#btnsearchaccnum").prop("disabled", false);
            $("#btnsearchaccnum").removeClass("disabled");
            $("#btnsearchsourceaccnum").prop("disabled", false);
            $("#btnsearchsourceaccnum").removeClass("disabled");
        }
        $(".useDeal").show();
        $("#TransactionType").prop("disabled", true);
        $("#RateType").prop("disabled", true);
        $("#TransactionCurrency").prop("disabled", true);
        $("#CustomerCurrency").prop("disabled", true);
        $('#TransactionRate').attr("readonly", "readonly");
        $('#ValuePeriod').prop("disabled", true);
    }
    else {
        $(".useDeal").hide();
        $("#btnclearaccnum").prop("disabled", false);
        $("#btnclearaccnum").removeClass("disabled");
        $("#btnclearsourceaccnum").prop("disabled", false);
        $("#btnclearsourceaccnum").removeClass("disabled");
        $("#btnsearchaccnum").prop("disabled", false);
        $("#btnsearchaccnum").removeClass("disabled");
        $("#btnsearchsourceaccnum").prop("disabled", false);
        $("#btnsearchsourceaccnum").removeClass("disabled");
    }
}

function FirstLoadEdit() {
    $("#btnclearaccnum").prop("disabled", true);
    $("#btnclearaccnum").addClass("disabled");
    $("#btnclearsourceaccnum").prop("disabled", true);
    $("#btnclearsourceaccnum").addClass("disabled");
    $("#btnsearchaccnum").prop("disabled", true);
    $("#btnsearchaccnum").addClass("disabled");
    $("#btnsearchsourceaccnum").prop("disabled", true);
    $("#btnsearchsourceaccnum").addClass("disabled");

    $(".statementdoc").hide();
    $(".referencestatementdoc").hide();
    $(".referencetrandoc").hide();
    $(".referencetransformdoc").hide();
    $(".transform").hide();
    $(".trandoc").hide();

    var transnum = $("#TransNum").val()

    $.ajax({
        type: 'POST',
        url: 'JsOnFirstLoadEdit',
        data: { TransNum : transnum },
        cache: false,
        success: function (data) {
            if (data.HaveDocTransLink) {
                if (data.UnderLimit) {
                    $(".doctrans").hide();
                }
                else {
                    $(".doctrans").show();
                }

                $(".referencetrandoc").show();
                $("#linktodocumenttransaction").attr("href", data.DocTransLinkSave);
                $("#PrevRefDocumentTransactionName").val(data.DocTransLinkName);

                $(".lblNominalUnderlying").text(parseFloat(data.NominalUnderlying).toLocaleString('en-US'));
                $(".lblRemainingUnderlying").text(parseFloat(data.RemainingUnderlying).toLocaleString('en-US'));
            }

            if (data.HaveDocStatementLink) {
                $(".referencestatementdoc").show();
                $("#linktostatementletter").attr("href", data.DocStatementLinkSave);
                $("#PrevRefDocumentStatementName").val(data.DocStatementLinkName);
            }

            if (data.HaveTransFormulirLink) {
                $(".referencetransformdoc").show();
                $("#linktotransactionform").attr("href", data.TransFormulirLinkSave);
                $("#PrevRefTransactionFormName").val(data.TransFormulirLinkName);
            }
        }
    });
}

function checkDocumentRequirement() {
    if (firstview == false) {
        $(".statementdoc").hide();
        $(".referencestatementdoc").hide();
        $(".referencetrandoc").hide();
        $(".referencetransformdoc").hide();
        $(".transform").hide();
        $(".trandoc").hide();

        var accno = $("#AccNum").val();
        var amount = parseFloat($("#TransactionNominal").val().replaceAll(",", "").replaceAll(".", ","));
        var basecurrency = $("#TransactionCurrency").val();
        var countercurrency = $("#CustomerCurrency").val();
        var trantype = $("#TransactionType").val();
        var ratetype = $("#RateType").val();
        var exchangerate = parseFloat($("#TransactionRate").val().replaceAll(",", "").replaceAll(".", ","));
        var doctrans = $("#DocumentTransId").val();

        if (accno != "" && amount > 0 && exchangerate > 0 && basecurrency != "" && countercurrency != "") {
            var convertedamount = amount
            $.ajax({
                type: 'POST',
                url: 'JsOnCheckDocumentRequirement',
                data: { accno: accno, convertedamount: convertedamount, basecurrency: basecurrency, trantype: trantype, ratetype: ratetype, doctrans: doctrans },
                cache: false,
                success: function (data) {
                    if (data.UnderLimit) {
                        $(".doctrans").hide();
                    }
                    else {
                        $(".doctrans").show();
                    }
                    if (data.UnderLimit && !data.HaveStatementLetter) {
                        //under limit and do not have statement letter this month
                        $(".statementdoc").show();
                        $(".transform").show();
                    }
                    else if (data.UnderLimit && data.HaveStatementLetter) {
                        //under limit and have previous statement letter this month
                        $(".referencestatementdoc").show();
                        $(".transform").show();
                        $("#linktostatementletter").attr("href", data.StatementLetterLink);
                        $("#PrevRefDocumentStatementName").val(data.StatementLetterLinktoSave);
                    }
                    else if (!data.UnderLimit && !data.HaveNPWP) {
                        //over limit and do not have npwp
                        bootbox.alert("Transaction limit reach and customer does not have NPWP. <br>Please complete Customer NPWP data in core banking!");
                    }
                    else if (!data.UnderLimit && data.HaveNPWP && !data.UnderlyingisFinal) {
                        //under limit and have npwp and underlying temporary
                        $(".statementdoc").show();
                        $(".transform").show();
                        if (data.PassNominalUnderlying) {
                            $(".trandoc").show();
                            $(".referencetrandoc").hide();
                            $(".lblNominalUnderlying").text("");
                            $(".lblRemainingUnderlying").text("");
                        }
                        else {
                            $(".trandoc").hide();
                            $(".referencetrandoc").show();
                            $(".lblNominalUnderlying").text(parseFloat(data.NominalUnderlying).toLocaleString('en-US'));
                            $(".lblRemainingUnderlying").text(parseFloat(data.RemainingUnderlying).toLocaleString('en-US'));

                            $("#linktodocumenttransaction").attr("href", data.DocTransLinkSave);
                            $("#PrevRefDocumentTransactionName").val(data.DocTransLinkName);
                        }
                    }
                    else if (!data.UnderLimit && data.HaveNPWP && data.UnderlyingisFinal && !data.HaveStatementLetter) {
                        //under limit and have npwp and underlying final and do not have statement letter
                        $(".statementdoc").show();
                        $(".transform").show();
                        if (data.PassNominalUnderlying) {
                            $(".trandoc").show();
                            $(".referencetrandoc").hide();
                            $(".lblNominalUnderlying").text("");
                            $(".lblRemainingUnderlying").text("");
                        }
                        else {
                            $(".trandoc").hide();
                            $(".referencetrandoc").show();
                            $(".lblNominalUnderlying").text(parseFloat(data.NominalUnderlying).toLocaleString('en-US'));
                            $(".lblRemainingUnderlying").text(parseFloat(data.RemainingUnderlying).toLocaleString('en-US'));

                            $("#linktodocumenttransaction").attr("href", data.DocTransLinkSave);
                            $("#PrevRefDocumentTransactionName").val(data.DocTransLinkName);
                        }
                    }
                    else if (!data.UnderLimit && data.HaveNPWP && data.UnderlyingisFinal && data.HaveStatementLetter) {
                        //under limit and have npwp and underlying final and have statement letter
                        $(".referencestatementdoc").show();
                        $(".transform").show();
                        $("#linktostatementletter").attr("href", data.StatementLetterLink);
                        $("#PrevRefDocumentStatementName").val(data.StatementLetterLinktoSave);
                        if (data.PassNominalUnderlying) {
                            $(".trandoc").show();
                            $(".referencetrandoc").hide();
                            $(".lblNominalUnderlying").text("");
                            $(".lblRemainingUnderlying").text("");
                        }
                        else {
                            $(".trandoc").hide();
                            $(".referencetrandoc").show();
                            $(".lblNominalUnderlying").text(parseFloat(data.NominalUnderlying).toLocaleString('en-US'));
                            $(".lblRemainingUnderlying").text(parseFloat(data.RemainingUnderlying).toLocaleString('en-US'));

                            $("#linktodocumenttransaction").attr("href", data.DocTransLinkSave);
                            $("#PrevRefDocumentTransactionName").val(data.DocTransLinkName);
                        }
                    }
                }
            });
        }
    }
}

$("#TransactionRate, #TransactionNominal").change(function () {
    calculateConvertion($("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#CustomerCurrency").val(), $("#TransactionNominal").val(), $("#TransactionRate").val())
    checkDocumentRequirement();
});

$("#TransactionType, #RateType, #TransactionCurrency, #CustomerCurrency").change(function () {
    if (checkrate) {   
        calculateConvertionWithGetRate($("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#CustomerCurrency").val(), $("#TransactionNominal").val())
    }
    else {
        calculateConvertion($("#TransactionType").val(), $("#RateType").val(), $("#TransactionCurrency").val(), $("#CustomerCurrency").val(), $("#TransactionNominal").val(), $("#TransactionRate").val())
    }
    checkDocumentRequirement();
});


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
function DelDocument(FilePath, ButtonValue) {
    $.ajax({
        type: 'POST',
        url: 'DeleteDocument',
        data: { FilePath: FilePath, ButtonValue: ButtonValue },
        cache: false,
        success: function (result) {
            if(result.Deleted == true){
                if (ButtonValue == 'DeleteDocumentTransaction') {
                    $(".referencetrandoc").hide();
                    $(".trandoc").show();
                }
                if (ButtonValue == 'DeleteDocumentStatement') {
                    $(".referencestatementdoc").hide();
                    $(".statementdoc").show();
                }
                if (ButtonValue == 'DeleteTransactionForm') {
                    $(".referencetransformdoc").hide();
                    $(".transform").show();
                }
            }
            else {
                bootbox.alert("File not found in this directory");
            }
            
        }
    });
};

$(function () {
    $("body").on("click", "#BtnSubmit", function (e) {
        e.preventDefault();
        if ($(".statementdoc").is(":visible") && !$("#FileDocumentStatement").val())
        {
            bootbox.alert("Please upload Letter of Statement");
            return;
        }
        
        if ($(".transform").is(":visible") && !$("#FileTransFormulir").val()) {
            bootbox.alert("Please upload Transaction Form");
            return;
        }

        if ($(".trandoc").is(":visible") && !$("#FileDocumentTransaction").val()) {
            bootbox.alert("Please upload Transaction Document");
            return;
        }
        
        $("#TransactionType").removeAttr("disabled");
        $("#RateType").removeAttr("disabled");
        $("#TransactionCurrency").removeAttr("disabled");
        $("#CustomerCurrency").removeAttr("disabled");
        $('#ValuePeriod').removeAttr("disabled");

        $("#frmData").submit();
    });

    $('body').on('click', '#btnsearchaccnum', function (e) {
        e.preventDefault();
        var link = $(this).attr('href');
        $(this).attr('href', link);
        $(this).attr('data-target', '#modal-container');
        $(this).attr('data-toggle', 'modal');
    });

    $('body').on('click', '#btnsearchsourceaccnum', function (e) {
        e.preventDefault();
        var link = $(this).attr('href');
        $(this).attr('href', link);
        $(this).attr('data-target', '#modal-container2');
        $(this).attr('data-toggle', 'modal');
    });

    $("body").on("click", "#btnclearaccnum", function (e) {
        e.preventDefault();
        $("#AccName").val("");
        $("#AccNum").val("")
    });

    $("body").on("click", "#btnclearsourceaccnum", function (e) {
        e.preventDefault();
        $("#SourceAccName").val("");
        $("#SourceAccNum").val("")
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

    $('body').on('click', '#DeleteDocStatement', function (e) {
        e.preventDefault();
        bootbox.dialog({
            message: "Are your sure to delete this letter of statement?",
            title: "Confirmation",
            buttons: {
                success: {
                    label: "Yes",
                    className: "btn-success",
                    callback: function () {
                        DelDocument($('#PrevRefDocumentStatementName').val(), $('#DeleteDocStatement').val());
                    }
                },
                danger: {
                    label: "No",
                    className: "btn-danger"
                }
            }
        });
    });

    $('body').on('click', '#DeleteTransactionForm', function (e) {
        e.preventDefault();
        bootbox.dialog({
            message: "Are your sure to delete this transaction formulir?",
            title: "Confirmation",
            buttons: {
                success: {
                    label: "Yes",
                    className: "btn-success",
                    callback: function () {
                        DelDocument($('#PrevRefTransactionFormName').val(), $('#DeleteTransactionForm').val());
                    }
                },
                danger: {
                    label: "No",
                    className: "btn-danger"
                }
            }
        });
    });

    $('body').on('click', '#DeleteDocumentTrans', function (e) {
        e.preventDefault();
        bootbox.dialog({
            message: "Are your sure to delete this document transaction?",
            title: "Confirmation",
            buttons: {
                success: {
                    label: "Yes",
                    className: "btn-success",
                    callback: function () {
                        DelDocument($('#PrevRefTransactionFormName').val(), $('#DeleteTransactionForm').val());
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
    

    $('body').on('click', '#btnadddeal', function (e) {
        e.preventDefault();
        if ($('#DealNumber').val() != '') {
            var dealnum = $('#DealNumber').val();
            $.ajax({
                url: 'JsOnCheckDealNumber',
                type: 'post',
                data: { dealnum : dealnum },
                dataType: 'json',
                success: function (data) {
                    var dealfound = false;
                    $('#TransDetail > tbody  > tr').each(function () {
                        if ($(this).find(".dealnumber").val() == dealnum)
                        {
                            dealfound = true;
                        }
                    });

                    if (dealfound)
                    {
                        bootbox.alert("Deal already registered");
                    }
                    else
                    {
                        if (!data.errMessage) {
                            bootbox.dialog({
                                message: "Are your sure to add this deal: <br><br> Deal Number: " + data.dealNumber + "<br>Customer: " + data.accNumber + " - " + data.accName + "<br>Transaction Type: " + data.tranType + " - " + data.dealType + "<br>Currency: " + data.fromCurrency + " to " + data.toCurrency + ", Rate: " + data.strexchRate + "<br>Amount: " + data.strdealAmount + "<br>",
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
                                            SetControl(true);

                                            var tid = parseInt($('#TransDetail tr:last').attr('id'));

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
});

function ChangeText(FileName, Target) {
    var FileInput = FileName.value.toString();
    var strFileName = FileInput;
    document.getElementById(Target).value = strFileName;
};

$('#ValuePeriod').change(function () {
    var valueType = $(this).val();

    $.ajax({
        url: 'JsOnGetValueDate',
        type: 'post',
        dataType: 'json',
        data: { valuetype: valueType },
        success: function (data) {
            $('#ValueDate').val(data.valueDate);
            if (valueType == "TOD") {
                try{
                    $("#ValueDate").datepicker("destroy");
                }
                catch(err){}
                $("#ValueDate").attr("style", "cursor:not-allowed")
            }
            else
            {
                $("#ValueDate").datepicker({
                    dateFormat: "dd-mm-yy",
                });
                try {
                    $("#ValueDate").datepicker("destroy");
                }
                catch (err) { }
                $("#ValueDate").attr("style", "cursor:not-allowed")
            }
        }
    });
});

$('#DocumentTransId').change(function () {
    var DocTrans = $(this).val();
    getLHBUDropdown(DocTrans);
});

function getLHBUDropdown(DocTrans) {
    $.ajax({
        url: 'JsOnGetLHBUDocument',
        type: 'post',
        dataType: 'json',
        data: { DocTrans: DocTrans },
        success: function (data) {
            var markup = "<option value=''>Select LHBU Document</option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].DocumentId + ">" + data[x].DocumentLHBUDisplay + "</option>";
            }
            $("#DocumentLHBUId").html(markup).show();
            $("#DocumentLHBUId").select2({ width: '100%' });
        }
    });

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
            alert('test');
            $("#PurposeId").html(markup).show();
            $("#PurposeId").select2({ width: '100%' });
        }
    });
}

String.prototype.replaceAll = function (str1, str2, ignore) {
    return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
}