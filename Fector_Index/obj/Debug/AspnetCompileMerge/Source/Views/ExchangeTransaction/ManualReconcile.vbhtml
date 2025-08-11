@ModelType Fector_Index.ManualReconcileViewModel

@Code
    ViewData("Title") = "Manual Reconcile"
    Dim Branch As SelectList = ViewBag.Branch
    Dim Currencies As SelectList = ViewBag.Currencies
End Code

<div class="col-md-12">
    @Using Html.BeginForm("ManualReconcile", "ExchangeTransaction", FormMethod.Post, New With {.class = "panel panel-default", .id = "frmData", .enctype = "multipart/form-data"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Manual Reconcile</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body" id="pnl1">
             <div class="row">
                 <div class="col-md-6">
                     @Html.ValidationSummary(True, "", New With { .class = "text-danger" })
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.DealNumber)</label>
                     @Html.CustomTextBoxFor(Function(f) f.DealNumber, New With {.class = "form-control", .placeholder = "Deal Number"})
                     @Html.ValidationMessageFor(Function(f) f.DealNumber, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.TDate)</label>
                     @Html.CustomTextBoxFor(Function(f) f.TDate, New With {.class = "form-control datetime-picker", .placeholder = "Date"})
                     @Html.ValidationMessageFor(Function(f) f.TDate, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.CoreReferenceNumber)</label>
                     @Html.CustomTextBoxFor(Function(f) f.CoreReferenceNumber, New With {.class = "form-control", .placeholder = "Reference Number"})
                     @Html.ValidationMessageFor(Function(f) f.CoreReferenceNumber, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.BranchId)</label>
                     @Html.DropDownListFor(Function(f) f.BranchId, Branch, "-- SELECT BRANCH --", New With {.class = "form-control", .placeholder = "Branch"})
                     @Html.ValidationMessageFor(Function(f) f.BranchId, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.AccountNumber)</label>
                     <div class="input-group">
                         @Html.CustomTextBoxFor(Function(f) f.AccountNumber, New With {.class = "form-control", .id = "AccNum", .placeholder = "Account Number", .readonly = "readonly"})
                         <span class="input-group-btn">
                             <a href="#" class="modal-link btn btn-primary" id="btnclearaccnum" title="Clear"><i class='fa fa-refresh'></i></a>
                             <a href="@Url.Action("BrowseAccNum", "TransactionDeal")" class="modal-link btn btn-primary" id="btnsearchaccnum" title="Browse"><i class='fa fa-search'></i></a>
                         </span>
                     </div>
                     @Html.ValidationMessageFor(Function(f) f.AccountNumber, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.AccountName)</label>
                     @Html.CustomTextBoxFor(Function(f) f.AccountName, New With {.class = "form-control", .id = "AccName", .placeholder = "Account Name", .readonly = "readonly"})
                     @Html.ValidationMessageFor(Function(f) f.AccountName, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.Currency)</label>
                     @Html.DropDownListFor(Function(m) m.Currency, Currencies, "-- SELECT CURRENCY --", New With {.class = "form-control", .placeholder = "Currency"})
                     @Html.ValidationMessageFor(Function(f) f.Currency, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.Time)</label>
                     @Html.CustomTextBoxFor(Function(f) f.Time, New With {.class = "form-control time-picker", .placeholder = "Time"})
                     @Html.ValidationMessageFor(Function(f) f.Time, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.Amount)</label>
                     @Html.CustomTextBoxFor(Function(f) f.Amount, New With {.class = "form-control autoNumeric", .placeholder = "Amount"})
                     @Html.ValidationMessageFor(Function(f) f.Amount, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button type="submit" id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="clearfix"></div>
    End Using
    <style>
        .modal-content {
            position: fixed;
            left: 0px;
            right: 0px;
            width: 800px !important;
            height: auto !important;
            margin: auto auto !important;
        }
    </style>
    <div id="modal-container" class="modal fade" tabindex="-1" role="dialog">
        <div class="modal-content">
        </div>
    </div>
</div>

@Section Scripts
<script>
    if ('@ViewBag.Init' != "true")
    {
        $TDate = $('input[name=TDate]');
        var defaultValue = $TDate.val();
        $TDate.val(ymdToDmy(defaultValue));
    }


    $(document).ready(function () {

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

        if ('@ViewBag.Init' == "true")
        {
            initializeComponentValue();
        }

        //Prevent from submit by hit enter
        $(window).keydown(function (event) {
            var modalClass = $('#modal-container').attr('class');
            if (event.keyCode == 13 && !modalClass.includes("in")) {
                event.preventDefault();
                return false;
            }
        });

        //Reverse from DMY to YMD
        $('#frmData').submit(function (event) {
            event.preventDefault();

            $TDate = $('input[name=TDate]');
            var defaultValue = $TDate.val();

            $TDate.val(dmyToYmd(defaultValue));
            $(this).unbind('submit').submit();

            $TDate.val(defaultValue);
        });
    });
</script>
End Section