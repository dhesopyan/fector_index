@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>Transaction Reconcile</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-5"><label>Fector Transaction</label></div>
                <div class="col-md-5"><label>Core Transaction</label></div>
                <div class="col-md-2">&nbsp;</div>
            </div>
            <div class="row">
                <div class="col-md-5">
                    <table class="table table-responsive table-reconcile" style="border:solid 1px">
                        <tbody>
                            @For Each trx As Fector_Index.ReconcileViewModel.ReconsileTrx In Model.PendingTransaction
                                @<tr>
                                     <td width="10%" style="text-align:center; vertical-align:top;">
                                         @Html.RadioButton("SelectedTransaction", trx.TransNum & "|" & trx.DealNumber, New With {.class = "customcheckbox", .id = "SelectedTransaction"})
                                     </td>
                                     <td width="30%">Deal #</td>
                                     <td width="2px">:</td>
                                    <td>@trx.DealNumber</td>
                                </tr>
                                @<tr>
                                    <td>&nbsp;</td>
                                    <td>Acc #</td>
                                    <td>:</td>
                                    <td>@trx.AccNum - @trx.AccName</td>
                                </tr>
                                @<tr>
                                    <td>&nbsp;</td>
                                    <td>Trx Type</td>
                                    <td>:</td>
                                    <td>@trx.RateType - @trx.TransactionType </td>
                                </tr>
                                @<tr>
                                    <td>&nbsp;</td>
                                    <td>Rate</td>
                                    <td>:</td>
                                    <td>@trx.TransactionCurrency - @trx.CustomerCurrency, @trx.TransactionRate.Value.ToString("N4")</td>
                                </tr>
                                @<tr>
                                    <td>&nbsp;</td>
                                    <td>Amount</td>
                                    <td>:</td>
                                    <td>@trx.TransactionNominal.Value.ToString("N2") - @trx.CustomerNominal.Value.ToString("N2")</td>
                                </tr>
                                @<tr>
                                    <td>&nbsp;</td>
                                    <td>Source Funds</td>
                                    <td>:</td>
                                    <td>@trx.SourceFunds @IIf(IsNothing(trx.SourceAccNum), "", "(" & trx.SourceAccNum & ")") - @trx.SourceNominal.Value.ToString("N2")</td>
                                </tr>
                            Next
                        </tbody>
                    </table>
                </div>
                <div class="col-md-5">
                    <table class="table table-responsive table-reconcile" style="border:solid 1px">
                        <tbody>
                            @For Each trx As Fector_Index.CoreTransaction In Model.CoreUnmapped
                                @<tr>
                                    <td width="10%" style="text-align:center; vertical-align:top;">@Html.RadioButton("SelectedCoreTrx", trx.Refno, New With {.class = "customcheckbox", .id = "SelectedCoreTrx"})</td>
                                    <td width="30%">Trx #</td>
                                    <td width="2px">:</td>
                                    <td>
                                        @trx.Refno
                                    </td>
                                </tr>
                                @<tr>
                                    <td>&nbsp;</td>
                                    <td>Time</td>
                                    <td>:</td>
                                    <td>@trx.TimeStr</td>
                                </tr>
                                @<tr>
                                    <td>&nbsp;</td>
                                    <td>Acc #</td>
                                    <td>:</td>
                                    <td>@trx.AccNo</td>
                                </tr>
                                @<tr style="border-bottom:dashed 1px">
                                    <td>&nbsp;</td>
                                    <td>Amount</td>
                                    <td>:</td>
                                    <td>@trx.CoreCurrId - @trx.Amount.Value.ToString("N2")</td>
                                </tr>
                            Next
                        </tbody>
                    </table>
                </div>
                <div class="col-md-2">
                    <div class="row">
                        <div class="col-md-12">
                            <form id="frmReconcile" action='@Url.Content("~")Reconcile/ReconcileTrx' method="post" class="btn btn-success btn-block btn-md" title="Reconcile">
                                <i class="fa fa-chain"></i> Map Trx
                                <input id="FectorTransNum" name="FectorTransNum" type="hidden" value="">
                                <input id="FectorDealNum" name="FectorDealNum" type="hidden" value="">
                                <input id="RecCoreRefno" name="RecCoreRefno" type="hidden" value="">
                            </form>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <form id="frmUnreconcile" action='@Url.Content("~")Reconcile/UnreconcileTrx' method="post" class="btn btn-success btn-block btn-md" title="Un-Reconcile">
                                <i class="fa fa-chain-broken"></i> Un-Map Trx
                                <input id="UnmCoreRefno" name="UnmCoreRefno" type="hidden" value="">
                            </form>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <form id="frmDelete" action='@Url.Content("~")Reconcile/DeleteCore' method="post" class="btn btn-warning btn-block btn-md" title="Delete">
                                <i class="fa fa-trash-o"></i> Delete Core Trx
                                <input id="DelCoreRefno" name="DelCoreRefno" type="hidden" value="">
                            </form>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <form id="frmReactivate" action='@Url.Content("~")Reconcile/Reactivate' method="post" class="btn btn-info btn-block btn-md" title="Reactivate">
                                <i class="fa fa-refresh"></i> Reactivate Trx
                                <input id="ReaCoreRefno" name="ReaCoreRefno" type="hidden" value="">
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
@Section scripts
    <script>
        $('body').on('click', '#frmReactivate', function (e) {
            e.preventDefault();
            var obj = $(this);

            bootbox.prompt("Please enter Core Ref #:", function (result) {
                if (result === null) {
                    
                } else {
                    $('#ReaCoreRefno').val(result);
                    obj.submit();
                }
            });
        });

        $('body').on("click", "#frmDelete", function (e) {
            e.preventDefault();
            var obj = $(this);

            var selectedVal = "";
            var selected = $("#SelectedCoreTrx:checked");
            selectedVal = selected.val();

            $('#DelCoreRefno').val(selectedVal);
            if (selected.length > 0) {
                bootbox.confirm("Are you sure want to delete selected core transaction data?", function (result) {
                    if (result) {
                        obj.submit();
                    }
                });
            }
            else {
                bootbox.alert("Please select core transaction to be deleted");
            }

        });

        $(document).on("click", "#frmUnreconcile", function (e) {
            e.preventDefault();
            var obj = $(this);

            bootbox.prompt("Please enter Core Ref # to Unmap:", function (result) {
                if (result === null) {

                } else {
                    $('#UnmCoreRefno').val(result);
                    obj.submit();
                }
            });
        });

        $(document).on("click", "#frmReconcile", function (e) {
            e.preventDefault();
            var obj = $(this);

            var selectedcoreVal = "";
            var selectedcore = $("#SelectedCoreTrx:checked");
            selectedcoreVal = selectedcore.val();

            var selectedtrxVal = "";
            var selectedtrx = $("#SelectedTransaction:checked");
            selectedtrxVal = selectedtrx.val();
            
            if (selectedcore.length > 0 && selectedtrx.length > 0) {
                $('#RecCoreRefno').val(selectedcoreVal);

                var trxid = selectedtrxVal.split("|");
                $('#FectorTransNum').val(trxid[0]);
                $('#FectorDealNum').val(trxid[1]);
                bootbox.confirm("Are you sure want to reconcile selected transaction to selected core transaction?", function (result) {
                    if (result) {
                        obj.submit();
                    }
                });
            }
            else {
                bootbox.alert("Please select Fector transaction and Core transaction to be reconciled");
            }
                
        });
    </script>
End Section