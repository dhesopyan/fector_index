@ModelType Fector_Index.DailyProfitViewModel
@Code
    ViewData("Title") = "Daily Profit"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim CountDealProfit As Integer = 0
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "DailyProfit", FormMethod.Post, New With {.class = "panel panel-default", .id = "frmData", .defaultButton = "BtnSave"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Daily Profit</h4>
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
                        <div class="form-group">
                            <a id="BtnSearch" class="btn btn-primary" href="#"><i class="fa fa-search"></i> Search</a>
                            <span class="help-block"></span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-12">
                    <table id="DailyProfit" class="table FectorTable">
                        <thead>
                            <tr id="0" class="GvHeader">
                                <th width="15%" class="hidden">
                                    ID
                                </th>
                                <th width="13%">
                                    Deal Date
                                </th>
                                <th width="20%">
                                    Deal Number
                                </th>
                                <th width="10%">
                                    Acc. Num.
                                </th>
                                <th width="20%">
                                    Acc. Name
                                </th>
                                <th width="10%">
                                    Currency
                                </th>
                                <th width="17%">
                                    Amount Deal
                                </th>
                                <th width="15%">
                                    Deal Rate
                                </th>
                                <th width="20%">
                                    BEP
                                </th>
                                <th width="20%">
                                    Branch Profit
                                </th>
                                <th width="20%">
                                    Grand Total
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            @If Not IsNothing(Model.ListDailyProfit) Then
                            CountDealProfit = 1
                            Dim rowid As Integer = 1
                            Dim id As Integer = rowid - 1
                                @For Each dtl As Fector_Index.DealWithProfit In Model.ListDailyProfit
                                    @<tr id="@(CInt(rowid))">
                                        <td class="hidden">
                                            @dtl.ID
                                            <input type='hidden' id='ListDailyProfit.Index' name='ListDailyProfit.Index' value='@CInt(id)' />
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).ID)
                                        </td>
                                        <td>
                                            @CDate(dtl.DealDate).ToString("dd/MM/yyyy")
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).DealDate)
                                        </td>
                                        <td>
                                            @dtl.DealNumber 
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).DealNumber)
                                        </td>
                                        <td>
                                            @dtl.AccNum
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).AccNum)
                                        </td>
                                        <td>
                                            @dtl.AccName 
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).AccName)
                                        </td>
                                        <td>
                                            @dtl.CurrencyDeal 
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).CurrencyDeal)
                                        </td>
                                        <td>
                                            @dtl.AmountDeal
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).AmountDeal)
                                        </td>
                                        <td>
                                            @dtl.DealRate
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).DealRate)
                                        </td>
                                        <td>
                                            @dtl.BEP
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).BEP)
                                        </td>
                                        <td>
                                            @dtl.BranchProfit
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).BranchProfit)
                                        </td>
                                        <td>
                                            @dtl.GrandTotal
                                            @Html.HiddenFor(Function(f) f.ListDailyProfit(CInt(id)).GrandTotal)
                                        </td>
                                    </tr>
                                rowid += 1
                                id = rowid - 1
                                Next
                                    Else
                            CountDealProfit = 0
                            End If
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSave" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("DailyProfit", "Index")"><i class="fa fa-arrow-left"></i> Back</a>
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
        //var baseUrl = '@Url.Content("~")';

        $('#BtnSearch').click(function () {
            
            var startperiod = $('#StartPeriod').val();
            var endperiod = $('#EndPeriod').val();

            $.ajax({
                type: 'POST',
                url: '@Url.Action("JsOnDailyProfit")',
                data: { startperiod: startperiod, endperiod: endperiod },
                dataType: 'json',
                cache: false,
                success: function (data) {
                    var tablerow = "";

                    for (var x = 0; x < data.length; x++) {
                        tid = parseInt($('#DailyProfit tr:last').attr('id'));

                        tid = tid + 1;
                        var rid = tid - 1;
                        var d = new Date(data[x][1]);

                        var month = d.getMonth() + 1;
                        var day = d.getDate();

                        var output = (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + d.getFullYear();

                        tablerow = "<tr id='" + tid + "'>";
                        tablerow += "<td class=hidden>";
                        tablerow += data[x][0];
                        tablerow += "<input type='hidden' id='ListDailyProfit.Index' name='ListDailyProfit.Index' value='" + rid + "' />";
                        tablerow += "<input type='hidden' id='ListDailyProfit[" + rid + "].ID' name='ListDailyProfit[" + rid + "].ID' value='" + data[x][0] + "' />";
                        tablerow += "</td>";
                        tablerow += "<td>";
                        tablerow += output;
                        tablerow += "<input type='hidden' id='ListDailyProfit[" + rid + "].DealDate' name='ListDailyProfit[" + rid + "].DealDate' value='" + data[x][1] + "' />";
                        tablerow += "</td>";
                        tablerow += "<td>";
                        tablerow += data[x][2];
                        tablerow += "<input type='hidden' id='ListDailyProfit[" + rid + "].DealNumber' name='ListDailyProfit[" + rid + "].DealNumber' value='" + data[x][2] + "' />";
                        tablerow += "</td>";
                        tablerow += "<td>";
                        tablerow += data[x][3];
                        tablerow += "<input type='hidden' id='ListDailyProfit[" + rid + "].AccNum' name='ListDailyProfit[" + rid + "].AccNum' value='" + data[x][3] + "' />";
                        tablerow += "</td>";
                        tablerow += "<td>";
                        tablerow += data[x][4];
                        tablerow += "<input type='hidden' id='ListDailyProfit[" + rid + "].AccName' name='ListDailyProfit[" + rid + "].AccName' value='" + data[x][4] + "' />";
                        tablerow += "</td>";
                        tablerow += "<td>";
                        tablerow += data[x][5];
                        tablerow += "<input type='hidden' id='ListDailyProfit[" + rid + "].CurrencyDeal' name='ListDailyProfit[" + rid + "].CurrencyDeal' value='" + data[x][5] + "' />";
                        tablerow += "</td>";
                        tablerow += "<td>";
                        tablerow += data[x][6];
                        tablerow += "<input type='hidden' class='amountdeal' id='ListDailyProfit[" + rid + "].AmountDeal' name='ListDailyProfit[" + rid + "].AmountDeal' value='" + data[x][6] + "' />";
                        tablerow += "</td>";
                        tablerow += "<td>";
                        tablerow += data[x][7];
                        tablerow += "<input type='hidden' class='dealrate' id='ListDailyProfit[" + rid + "].DealRate' name='ListDailyProfit[" + rid + "].DealRate' value='" + data[x][7] + "' />";
                        tablerow += "</td>";
                        tablerow += "<td>";
                        if (data[x][5] != 'USD') {
                            tablerow += "<input type='text' class='bep form-control' id='ListDailyProfit[" + rid + "].BEP' name='ListDailyProfit[" + rid + "].BEP' value='" + data[x][8] + "' />";
                        }
                        else {
                            tablerow += "<input type='text' class='bep form-control' disabled id='ListDailyProfit[" + rid + "].BEP' name='ListDailyProfit[" + rid + "].BEP' value='" + data[x][8] + "' />";
                        }
                        tablerow += "</td>";
                        tablerow += "<td>";
                        if (data[x][5] != 'USD') {
                            tablerow += "<input type='text' class='branchprofit form-control' disabled id='ListDailyProfit[" + rid + "].BranchProfit' name='ListDailyProfit[" + rid + "].BranchProfit' value='" + data[x][9] + "' />";
                        }
                        else {
                            tablerow += "<input type='text' class='branchprofit form-control' id='ListDailyProfit[" + rid + "].BranchProfit' name='ListDailyProfit[" + rid + "].BranchProfit' value='" + data[x][9] + "' />";
                        }
                        tablerow += "</td>";
                        tablerow += "<td>";
                        tablerow += "<input type='text' class='grandtotal form-control' readonly id='ListDailyProfit[" + rid + "].GrandTotal' name='ListDailyProfit[" + rid + "].GrandTotal' value='" + data[x][10] + "' />";
                        tablerow += "</td>";
                        tablerow += "</tr>";

                        $('#DailyProfit > tbody:last-child').append(tablerow);
                    }
                }
            });
        });
        $('.branchprofit').change(function () {
            
            var branchprofit = $('.branchprofit').val();
            var amountdeal = $('.amountdeal').val();
            var subtotal = branchprofit * amountdeal;
            console.log(branchprofit);
            $('.grandtotal').val(subtotal);
        });

        $('body').on('change', '.branchprofit', function () {
            var trId = $(this).closest('tr').prop('id');
            trId = trId - 1;

            var amountdeal = document.getElementById('ListDailyProfit[' + trId + '].AmountDeal').value.replace(',','');
            document.getElementById('ListDailyProfit[' + trId + '].GrandTotal').value = parseFloat($(this).val() * amountdeal).toLocaleString("en-US");
        });

        $('body').on('change', '.bep', function () {
            var trId = $(this).closest('tr').prop('id');
            trId = trId - 1;

            var amountdeal = document.getElementById('ListDailyProfit[' + trId + '].AmountDeal').value.replace(',', '');
            var dealrate = document.getElementById('ListDailyProfit[' + trId + '].DealRate').value.replace(',', '');
            var bep = $(this).val();

            if (bep == 0) {
                document.getElementById('ListDailyProfit[' + trId + '].GrandTotal').value = parseFloat('0').toLocaleString("en-US");
            }
            else {
                document.getElementById('ListDailyProfit[' + trId + '].GrandTotal').value = parseFloat((((bep - dealrate) * - 1) * amountdeal)/2).toLocaleString("en-US");
            }
        });
    </script>

    End Section
</div>
