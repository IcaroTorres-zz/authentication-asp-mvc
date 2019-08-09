$(document).ready(function () {
    const maskArray = [
        { seletor: '.tel', mask: '(99) 9999-99999' },
        { seletor: '.cpf', mask: '999.999.999-99', reverse: true },
        { seletor: '.cep', mask: '99999-999' },
        { seletor: '.birth', mask: '99/99/9999' },  
        {
            seletor: '.money',
            mask: '[999]9,99',
            groupSeparator: '.',
            digits: 2,
            radixPoint: ",",
            allowMinus: false,
            placeholder: '0,00',
            removeMaskOnSubmit: true,
            numericInput: true
        },
        { seletor: '.ip', mask: '999.999.999.999' }
    ];
    maskArray.forEach(function (obj) {
        //console.log(obj);
        $(obj.seletor).inputmask(obj.alias, obj);
    });
});