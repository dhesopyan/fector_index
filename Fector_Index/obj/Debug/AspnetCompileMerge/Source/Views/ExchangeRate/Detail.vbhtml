@ModelType Fector_Index.MsExchangeRate 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim Currencies As SelectList = ViewBag.CurrencyList
End Code
<div class="col-md-12">
    @Using Html.BeginForm(Action, "ExchangeRate", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Extract Setting Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TTime)</label>
                    <input type="text" class="form-control" value="Now" readonly />
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.CurrId)</label>
                    @Html.DropDownListFor(Function(f) f.CurrId, Currencies, "-- SELECT CURRENCY --", New With {.class = "form-control", .placeholder = "Currency"})
                    @Html.ValidationMessageFor(Function(f) f.CurrId, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TTBuyRate)</label>
                    @Html.CustomTextBoxFor(Function(f) f.TTBuyRate, New With {.class = "form-control autoNumeric", .placeholder = "TT Buy Rate"})
                    @Html.ValidationMessageFor(Function(f) f.TTBuyRate, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.TTSellRate)</label>
                    @Html.CustomTextBoxFor(Function(f) f.TTSellRate, New With {.class = "form-control autoNumeric", .placeholder = "TT Sell Rate"})
                    @Html.ValidationMessageFor(Function(f) f.TTSellRate, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.BNBuyRate)</label>
                    @Html.CustomTextBoxFor(Function(f) f.BNBuyRate, New With {.class = "form-control autoNumeric", .placeholder = "BN Buy Rate"})
                    @Html.ValidationMessageFor(Function(f) f.BNBuyRate, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.BNSellRate)</label>
                    @Html.CustomTextBoxFor(Function(f) f.BNSellRate, New With {.class = "form-control autoNumeric", .placeholder = "BN Sell Rate"})
                    @Html.ValidationMessageFor(Function(f) f.BNSellRate, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.ClosingRate)</label>
                    @Html.CustomTextBoxFor(Function(f) f.ClosingRate, New With {.class = "form-control autoNumeric", .placeholder = "Closing Rate"})
                    @Html.ValidationMessageFor(Function(f) f.ClosingRate, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "ExchangeRate")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
    @Section scripts

    End Section
</div>