
//input text validation

try {
    $('.file-action').hide();
} catch (e) {

}
try {
    $('#minmax').click(function () {
        if (this.classList.contains('affiche')) {
            this.classList.remove('affiche')
            $('.ligne1').removeClass('col-4');
            $('.ligne1').addClass('col-3');
            $('.minmax').removeClass('col-12');
            $('.minmax').addClass('col-6');
            $('#minmax').removeClass('col-3');
            $('#minmax').addClass('col-6');
            $('#pan-montant').removeClass('col-3');
            $('#pan-montant').addClass('col-6');
            $('#pan-minmax').addClass('row');
            $('#maxmontant').show();
            $('#minmontant-lab').text(" Montant minimum");
        } else {
            this.classList.add('affiche')
            $('#maxmontant').hide();
            $('.ligne1').addClass('col-4');
            $('.ligne1').removeClass('col-3');
            $('.minmax').addClass('col-12');
            $('.minmax').removeClass('col-6');
            $('#minmax').addClass('col-3');
            $('#minmax').removeClass('col-6');
            $('#pan-montant').addClass('col-3');
            $('#pan-montant').removeClass('col-6');
            $('#pan-minmax').removeClass('row');
            $('#minmontant-lab').text(" Montant");
        }
    })
} catch (e) {

}
try {
    $('.pan-actions').hide();
    $('._legende').parent().addClass('ouvre');
    $('._legende').parent().removeClass('border');
    $('._legende').click(function () {
        if (this.classList.contains('ouvre')) {
            $('#' + this.getAttribute('_id')).show(100);
            this.classList.remove('ouvre');
            this.classList.remove('mdi-chevron-right-box');
            this.classList.add('mdi-chevron-left-box-outline');
            this.parentElement.classList.add('border');
        } else {
            $('#' + this.getAttribute('_id')).hide(100);
            this.classList.add('ouvre');
            this.classList.add('mdi-chevron-right-box');
            this.classList.remove('mdi-chevron-left-box-outline');
            this.parentElement.classList.remove('border');
        }
    });
} catch (e) {

}
try {
    $('#plus-info-input').on('input', function () {
        if (this.checked) {
            $('.avance').show();
        } else {
            $('.avance').hide();
        }
    })
} catch (e) {

}
//Change vue
try {
    $('.vue-item').click(function () {
        let tablist = "box";
        var _url = window.location.href;
        try {
            _url = _url.replace('&tablist=box', '');
        } catch (e) {

        }
        try {
            _url = _url.replace('&tablist=list', '');
        } catch (e) {

        }
        if (this.getAttribute('vue') == "list") {
            tablist = "list";
        } else {
            tablist = "box";
        }
        document.getElementById('link-change-vue').setAttribute('href', "" + _url + "&tablist=" + tablist);
        document.getElementById('link-change-vue').click();
    });
} catch (e) {

}

$(document).ready(function () {
    var tab = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.'];
    var oldval = "";
    try {
        //$(".valided").on('keyup', function (event) {
        //    if (tab.includes(event.key) && this.value.includes('.') && event.key == '.') {
        //        alert(event.key + " " + event.which);
        //        event.preventDefault();
        //    }
        //});
        $('.valided').on('input', function () {
            var val = this.value + "";
            this.value = ""
            for (var i = 0; i < val.length; i++) {
                try {
                    if (!tab.includes(val[i]) || (this.value.includes('.') && val[i] == '.')) {
                        continue;
                    }
                    this.value += val[i];
                } catch (e) {
                    alert(e)
                }
            }
        });
    } catch (e) {

    }
    //supprimer la derniere valeur si est virgule
    try {
        $('.valided').on('change', function () {
            var val = this.value + "";
            var index = Number(val.length - 1);
            if (val.charAt(index) == '.') {
                this.value = val.substr(0, Number(val.length - 1));
            }
            var reste = Number(val.split('.')[1]);
            if (reste == 0) {
                this.value = val.split('.')[0];
            }
        });
    } catch (e) {

    }
});