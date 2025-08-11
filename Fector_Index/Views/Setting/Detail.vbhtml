@ModelType Fector_Index.MsSettingViewModel 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    
    Dim BType As SelectList = ViewBag.BType
    Dim Status As SelectList = ViewBag.Status
    Dim CurrencyList As SelectList = ViewBag.Currency
    Dim Country As SelectList = ViewBag.Country
    
End Code

<div class="col-md-12">
    @Using Html.BeginForm("Edit", "Setting", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Setting Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.BankName)</label>
                    @Html.CustomTextBoxFor(Function(f) f.BankName, New With {.class = "form-control", .placeholder = "Bank Name"})
                    @Html.ValidationMessageFor(Function(f) f.BankName, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.BankBICode)</label>
                    @Html.CustomTextBoxFor(Function(f) f.BankBICode, New With {.class = "form-control", .placeholder = "BI Code"})
                    @Html.ValidationMessageFor(Function(f) f.BankBICode, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
             <div class="row">
                 <div class="form-group col-md-12">
                     <label>@Html.DisplayNameFor(Function(f) f.BankCountryID)</label>
                     @Html.DropDownListFor(Function(f) f.BankCountryID, Country, "-- SELECT COUNTRY --", New With {.class = "form-control", .placeholder = "Country"})
                     @Html.ValidationMessageFor(Function(f) f.BankCountryID, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-12">
                     <label>@Html.DisplayNameFor(Function(f) f.BankBusinessTypeID)</label>
                     @Html.DropDownListFor(Function(f) f.BankBusinessTypeID, BType, "-- SELECT BUSINESS TYPE --", New With {.class = "form-control", .placeholder = "Business Type"})
                     @Html.ValidationMessageFor(Function(f) f.BankBusinessTypeID, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-12">
                     <label>@Html.DisplayNameFor(Function(f) f.BankSubjectStatusID)</label>
                     @Html.DropDownListFor(Function(f) f.BankSubjectStatusID, Status, "-- SELECT SUBJECT STATUS --", New With {.class = "form-control", .placeholder = "Subject Status"})
                     @Html.ValidationMessageFor(Function(f) f.BankSubjectStatusID, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-4">
                     <label>@Html.DisplayNameFor(Function(f) f.TransactionLimit)</label>
                     @Html.DropDownListFor(Function(f) f.LimitCurrency, CurrencyList, "-- SELECT LIMIT CURRENCY --", New With {.class = "form-control", .placeholder = "Limit Currency"})
                     @Html.ValidationMessageFor(Function(f) f.LimitCurrency, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-8">
                     <label>&nbsp; </label>
                     @Html.CustomTextBoxFor(Function(f) f.TransactionLimit, New With {.class = "form-control autoNumeric", .placeholder = "Transaction Limit"})
                     @Html.ValidationMessageFor(Function(f) f.TransactionLimit, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "Home")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
</div>
