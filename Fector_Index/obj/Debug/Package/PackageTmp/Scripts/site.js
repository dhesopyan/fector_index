$(document).ready(function () {
    setAutoNumeric();
    $("#loading").hide()

    initializeComponent();
});

function initializeComponent ()
{
    //Initialize datetime-picker component
    $('.datetime-picker').datepicker({
        dateFormat: $(this).data('format') ? $(this).data('format') : 'dd-mm-yy',
    });

    //Initialize time-picker component
    $('.time-picker').datetimepicker({
        pickDate: false,
        Default: {
            upIcon: "fa fa-arrow-up",
            downIcon: "fa fa-arrow-down"
        },
        pickTime: true,
        format: 'hh:mm:ss'
    });
    
}

function dmyToYmd (str)
{
    str = str.split('-');
    return str[2] + '-' + str[1] + '-' + str[0];
}

function ymdToDmy(str) {
    str = str.split('-');
    return str[2] + '-' + str[1] + '-' + str[0];
}

function initializeComponentValue ()
{
    var now = new Date();
    var dateNow = leftPad(now.getDate()) + '-' + leftPad(now.getMonth() + 1) + '-' + now.getFullYear();
    $('.datetime-picker').val($(this).data('value') ? $(this).data('value') : dateNow);

    var timeNow = leftPad(now.getHours()) + ':' + leftPad(now.getMinutes()) + ':' + leftPad(now.getSeconds());
    $('.time-picker').val($(this).data('value') ? $(this).data('value') : timeNow);
}

function leftPad(n, digitLength) {
    digitLength = digitLength ? digitLength : 2;
    var output = n + '';
    while (output.length < digitLength) {
        output = '0' + output;
    }
    return output;
}

$(document).ajaxStart(function () {
    $.blockUI({
        centerY: 0,
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#fff',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: .5,
            color: '#000'
        },
        overlayCSS: {
            backgroundColor: '#000',
            opacity: 1.0,
            cursor: 'wait'
        },
    });
});

$(document).ajaxStop(function () {
    $.unblockUI();
});

function setAutoNumeric() {
    $('.autoNumeric').each(function () {
        var value = $(this).val();
        var maxValue = $(this).attr("autoNumeric-MaxValue");
        var minValue = $(this).attr("autoNumeric-MinValue");
        var decimalPlace = $(this).attr("autoNumeric-DecimalPlace");
        var thousandSeparator = $(this).attr("autoNumeric-ThousandSeparator");

        if (decimalPlace == null) {
            decimalPlace = 0;
        }
        if (maxValue == null) {
            maxValue = "9999999999999.99";
        }
        if (minValue == null) {
            minValue = "-9999999999999.99";
        }
        if (thousandSeparator == null) {
            thousandSeparator = ",";
        }
        value = value.replace(/,/g, "");
        $(this).val(value);

        $(this).autoNumeric('init', {
            mDec: 2,
            vMax: maxValue,
            vMin: minValue,
            aSep: thousandSeparator
        });

        if (value != null) {
            $(this).autoNumeric('set', value);
        }
    });

    $('.autoNumericRate').each(function () {
        var value = $(this).val();
        var maxValue = $(this).attr("autoNumeric-MaxValue");
        var minValue = $(this).attr("autoNumeric-MinValue");
        var decimalPlace = $(this).attr("autoNumeric-DecimalPlace");
        var thousandSeparator = $(this).attr("autoNumeric-ThousandSeparator");

        if (decimalPlace == null) {
            decimalPlace = 0;
        }
        if (maxValue == null) {
            maxValue = "9999999999999.9999";
        }
        if (minValue == null) {
            minValue = "-9999999999999.9999";
        }
        if (thousandSeparator == null) {
            thousandSeparator = ",";
        }
        value = value.replace(/,/g, "");
        $(this).val(value);

        $(this).autoNumeric('init', {
            mDec: 4,
            vMax: maxValue,
            vMin: minValue,
            aSep: thousandSeparator
        });

        if (value != null) {
            $(this).autoNumeric('set', value);
        }
    });
}
