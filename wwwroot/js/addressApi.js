const apiBase = "https://countriesnow.space/api/v0.1/countries";


$(document).ready(function () {
    // 1. Initial Load: Countries
    $.get(apiBase, function (res) {
        let dropdowns = $('#CountrySel, #ReceiverCountrySel');
        dropdowns.empty().append('<option value="">-- Select Country --</option>');
        res.data.forEach(c => {
            dropdowns.append(`<option value="${c.country}">${c.country}</option>`);
        });
    });


    // 2. Change Country -> Load Provinces/States
    $('#CountrySel, #ReceiverCountrySel').change(function () {
        let country = $(this).val();
        let isReceiver = $(this).attr('id').includes('Receiver');
        let provDrop = isReceiver ? $('#ReceiverProvinceSel') : $('#ProvinceSel');
        let cityDrop = isReceiver ? $('#ReceiverCitySel') : $('#CitySel');


        provDrop.prop('disabled', true).html('<option>Loading Provinces...</option>');
        cityDrop.prop('disabled', true).html('<option>Select Province First</option>');


        $.post(`${apiBase}/states`, { country: country }, function (res) {
            provDrop.empty().append('<option value="">-- Select Province --</option>').prop('disabled', false);
            res.data.states.forEach(s => {
                provDrop.append(`<option value="${s.name}">${s.name}</option>`);
            });
        }).fail(() => provDrop.html('<option value="N/A">N/A (Type in Street Address)</option>'));
    });


    // 3. Change Province -> Load Cities
    $('#ProvinceSel, #ReceiverProvinceSel').change(function () {
        let isReceiver = $(this).attr('id').includes('Receiver');
        let country = isReceiver ? $('#ReceiverCountrySel').val() : $('#CountrySel').val();
        let state = $(this).val();
        let cityDrop = isReceiver ? $('#ReceiverCitySel') : $('#CitySel');


        cityDrop.prop('disabled', true).html('<option>Loading Cities...</option>');


        $.post(`${apiBase}/state/cities`, { country: country, state: state }, function (res) {
            cityDrop.empty().append('<option value="">-- Select City --</option>').prop('disabled', false);
            res.data.forEach(c => {
                cityDrop.append(`<option value="${c}">${c}</option>`);
            });
        }).fail(() => cityDrop.html('<option value="N/A">Manual Entry Required</option>'));
    });
});
