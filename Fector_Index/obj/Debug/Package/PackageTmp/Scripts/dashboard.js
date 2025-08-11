$(document).ready(function () {
    LineBarExRate($('#ExRateCurr').val());
    LineBarDeal($('#ExRateDeal').val());
    LineBarTrans($('#ExRateTrans').val());
  })

function LineBarExRate(currId) {
    $.ajax({
        type: 'POST',
        url: 'Home/JsOnLineBarExRate',
        data: { currency: currId },
        dataType: 'json',
        success: function (data) {
            var result = [];
            for (var x = 0; x < data.length; x++) {
                result.push({
                    TTime: data[x][0],
                    BNSellRate: data[x][1],
                    BNBuyRate: data[x][2],
                    TTSellRate: data[x][3],
                    TTBuyRate: data[x][4]
                });
            }
            new Morris.Line({
                // ID of the element in which to draw the chart.
                element: 'divExchangeRate',
                // Chart data records -- each entry in this array corresponds to a point on
                // the chart.
                data: result,
                // The name of the data record attribute that contains x-values.
                xkey: 'TTime',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['BNSellRate', 'BNBuyRate', 'TTSellRate', 'TTBuyRate'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['BN Sell', 'BN Buy', 'TT Sell', 'TT Buy']
            });
        }
    });
};

function LineBarDeal(currId, branchId, startPeriod, endPeriod) {
    $.ajax({
        type: 'POST',
        url: 'Home/JsOnLineBarDeal',
        data: { currency: currId, branchId: branchId, startPeriod: startPeriod, endPeriod: endPeriod },
        dataType: 'json',
        success: function (data) {
            var result = [];
            for (var x = 0; x < data.length; x++) {
                result.push({
                    DealDate: data[x][0],
                    TotalDeal: data[x][1]
                });
            }
            new Morris.Line({
                // ID of the element in which to draw the chart.
                element: 'divDeal',
                // Chart data records -- each entry in this array corresponds to a point on
                // the chart.
                data: result,
                // The name of the data record attribute that contains x-values.
                xkey: 'DealDate',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['TotalDeal'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['Total Deal']
            });
        }
    });
};

function LineBarTrans(currId) {
    $.ajax({
        type: 'POST',
        url: 'Home/JsOnLineBarTrans',
        data: { currency: currId },
        dataType: 'json',
        success: function (data) {
            var result = [];
            for (var x = 0; x < data.length; x++) {
                result.push({
                    TDate: data[x][0],
                    TotalTrans: data[x][1]
                });
            }
            new Morris.Line({
                // ID of the element in which to draw the chart.
                element: 'divTrans',
                // Chart data records -- each entry in this array corresponds to a point on
                // the chart.
                data: result,
                // The name of the data record attribute that contains x-values.
                xkey: 'TDate',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['TotalTrans'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['Total Transaction']
            });
        }
    });
};


$("#ExRateCurr").change(function () {
    $('#divExchangeRate').empty();
    LineBarExRate($('#ExRateCurr').val());
});

$("#ExRateDeal").change(function () {
    $('#divDeal').empty();
    LineBarDeal($('#ExRateDeal').val(), $('#DealBranch').val(), $('#StartPeriod').val(), $('#EndPeriod').val());
});

$("#ExRateTrans").change(function () {
    $('#divTrans').empty();
    LineBarTrans($('#ExRateTrans').val());
});

$("#DealBranch").change(function () {
    $('#divTrans').empty();
    LineBarTrans($('#DealBranch').val());
});

$("#TransactionBranch").change(function () {
    $('#divTrans').empty();
    LineBarTrans($('#TransactionBranch').val());
});

$("#StartPeriod").change(function () {
    $('#divDeal').empty();
    LineBarDeal($('#ExRateDeal').val(), $('#DealBranch').val(), $('#StartPeriod').val(), $('#EndPeriod').val());
});

$("#EndPeriod").change(function () {
    $('#divDeal').empty();
    LineBarDeal($('#ExRateDeal').val(), $('#DealBranch').val(), $('#StartPeriod').val(), $('#EndPeriod').val());
});

