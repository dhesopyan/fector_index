@Code
    ViewData("Title") = "Exchange Rate List"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>@IIf(Action = "Index", "Exchange Rate List", "Exchange Rate Approval List")</h4>
            </div>
            <div class="pull-right">
                @If Action = "Index" Then
                    @<a href="~/ExchangeRate/Create" class="btn btn-primary"><i class="fa fa-plus"></i>&nbsp;Create</a>
                End If
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader" style="background-color: #f7f7f7">
                                <th class="hidden">
                                    RateId
                                </th>
                                <th class="filter" data-id="datefilter">
                                    TDate
                                </th>
                                <th class="filter" data-id="timepicker">
                                    TTime
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    <a id="BtnFilter" class="btn btn-primary btn-sm">
                                        <span class="fa fa-search"></span>
                                    </a>
                                    <a id="BtnClear" class="btn btn-primary btn-sm">
                                        <span class="fa fa-refresh"></span>
                                    </a>
                                </th>
                            </tr>
                            <tr id="GvHeader" class="GvHeader">
                                <th class="hidden" rowspan="2">
                                    RateId
                                </th>
                                <th rowspan="2" style="vertical-align :top;">
                                    TDate
                                </th>
                                <th rowspan="2" style="vertical-align :top;">
                                    TTime
                                </th>
                                <th rowspan="2" style="vertical-align :top;">
                                    Exchange Rate
                                </th>
                                <th colspan="2" style="text-align :center; width:250px;">
                                    TT
                                </th>
                                <th colspan="2" style="text-align :center; width:250px;">
                                    BN
                                </th>
                                <th rowspan="2" style="vertical-align :top;">
                                    Closing Rate
                                </th>
                                <th rowspan="2" style="vertical-align :top;">
                                    Status
                                </th>
                                <th rowspan="2">&nbsp;</th>
                            </tr>
                            <tr id="GvHeader2" style="background-color: #2C3E50">
                                <th>
                                    Sell
                                </th>
                                <th>
                                    Buy
                                </th>
                                <th>
                                    Sell
                                </th>
                                <th>
                                    Buy
                                </th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
<style>
    .dataTables_filter, .dataTables_info {
        display: none;
    }
</style>
@Section scripts
    @If Action = "Index" Then
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTExchangeRateAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/exchangerate-index.js")
    Else
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTExchangeRateApprovalAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/exchangerate-approval.js")
    End If

    <script>
        $(document).ready(function () {
            //var d = new Date();

            //var month = d.getMonth() + 1;
            //var day = d.getDate();

            //var output = (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + d.getFullYear();

            //$('#datefilter').val(output);
            $('#datefilter').datepicker({
                dateFormat: "dd-mm-yy",
            });

            $('#timepicker').datetimepicker({
                pickDate: false,

                Default: {
                    upIcon: "fa fa-arrow-up",
                    downIcon: "fa fa-arrow-down"
                },

                pickTime: true,
                format: 'hh:mm:ss'
            });
        });
    </script>
End Section
