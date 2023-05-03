$(document).ready(function () {

    //LOAD-More

    $(document).on("click", ".load-more", function () {

        let parent = $(".parent-products");

        let skipCount = $(parent).children().length;
        //skipCount=htmlde olan clasin childrenlerin sayi,lazimdi her appen eliyende negeder skip=yani  buraxaq deye

        let dataCount = $(parent).attr("data-count");

          //ajax-vasitesi ile request atiriq Urlere
        $.ajax({

             //urlde:-contoleri ve actionu yaziriq ve skip-adli varebla deyer gonderik,ordan gebul edib skip elesin deye
            url: `shop/loadmore?skip=${skipCount}`,
            //type:-datani gotururuk deye type=get
            type: "Get",
            //succsesden sonra hansi function islesin
            success: function (res) {

                $(parent).append(res);

                //skipCount-burada cagirirsan cunku yuxarda caqirib methoda gonderirik,ama appenden sonra reqem deyisir deye bura yeniden cagirmali oluruq
                skipCount = $(parent).children().length;

                if (skipCount >= dataCount) {
                    $(".load-more").addClass("d-none");
                    $(".show-less").removeClass("d-none");
                }
            }

        })
    })



    //SHOW LESS
    $(document).on("click", ".show-less", function () {

       //show-lesse-basanda butun productlar itsin esas olanlar gorsensin

        let skipCount = 0;


        $.ajax({
            url: `shop/loadmore?skip=${skipCount}`,
            type: "Get",
            success: function (res) {

               //html-""-bos edirikki kohnenin ustune yazdirmayaq esaslari
                $(".parent-products").html("");
                $(".parent-products").append(res);

                $(".load-more").removeClass("d-none");
                $(".show-less").addClass("d-none");
            }

        })
    })






    //SEARCH

    $(document).on("keyup", "#input-search", function () {   //search inputun keyupinda(elimizi qaldiranda) islesin
        $("#searchList li").slice(1).remove();

        let value = $("#input-search").val();  //inputun valusun elde edirik

        $.ajax({    //request getsin methoda
            url: "/shop/search?searchText=" + value,    //bu Urle getsin request ve inputun deyerini Actiona arqument kimi gonderiririk
            method: "Get",
            success: function (res) {
                $("#searchList").append(res);   //Ul-eye append edirik res(method bize PartilaView qaytarir)
            }
        })
    })








    //ADD PRODUCTS basket WITH reguest AJAX  (alaxla product u add edirikki sehife refrsh olmasin)

    $(document).on("click", ".add-basket", function () {   //add-basket butona basdiqda islesin

        debugger
        let productId = $(this).parent().attr("data-id")   //hemin add-basket butonuna aid olan productun Id-sini goturuk

        let data = { id: productId };
        //productumuzun id sini data adli variableye beraber edir bu wekilde gonderirik ajaxa


        $.ajax({
            url: `home/AddBasket`,     //add-basket Action request atiriq
            type: "post",
            data: data,                 //data ajaxsin ozunde olur gondereceyimiz data sayilir( yeni yuxaridaki variable deyil) ona beraber edirik data variableni gonderirik backende
            success: function (res) {
               
                $(".cart-count").text(res);
                //yuxari hissedeki basket cartin sayini artirmaq(amma sehife refresh olanda gedecek. asp ile viewcompanentle edeceyik refreshde getmesin deye).


                swal("Product added to basket", "", "success");  // sweet alert

            }
        })
        return false;
    })






    //DELETE PRODUCT FROM BASKET WITH AJAX

    $(document).on("click", ".delete-btn", function () {      //delete butona basdiqda islesin


        let deletProduct = $(this).parent().parent();                  //hemin delete edeceymiz productun row-nu goturuk (yeni tableni tr- sini)

        let productId = $(this).attr("data-id")                         //hemin delete butonuna aid olan productun Id-sini goturuk  (yeni tr-ye verdiyimiz data-id ni)

        let sum = 0;

        let grandTotal = $(".total-product").children().eq(0);          //umumi butun productlarin giymetin gotururuk( sildikden sonra deyishmek ucun)

        $.ajax({
            url: `card/DeleteProductFromBasket?id=${productId}`,       //productu basketden silen Actiona request atiriq
            type: "Post",
            success: function (res) {
                res--
                $(".cart-count").text(res);


                $(deletProduct).remove();                              //siline productun row-nu silirik (yeni tr-ni)
                for (const product of $(".table-product").children()) {     //tabledaki-rowlari(productlari) fora saliriq  (yeni tr-leri)
                    let total = parseFloat($(product).children().eq(6).text())   //butun productlarin her row uzre ara totalini gotururuk (her tr-de olan totali)
                    sum += total    //ara totallari toplayiriq
                }


                $(grandTotal).text(sum);   //ve umimi totala (ara totallarin cemini, yeni her rowda olan totallarin cemi) elave edirk


                swal("Product deleted to basket", "", "success");   // sweet alert


                if ($(".table-product").children().length == 0) {     //eyer tableda-row(product) yoxdursa 
                    $("table").addClass("d-none");                      //hemin table d-none edirik
                    $(".total-product").addClass("d-none");             //grand-totalida d-none edirik
                    $(".alert-product").removeClass("d-none");            //alertden d-none silirik
                }


            }
        })
        return false;
    })




    //Decrease Product from Basket (product sayini basketde azaltmaq )
    $(document).on("click", ".minus", function () {

        let productId = $(this).parent().parent().attr("data-id");     //hemin minusa aid olan productun Id-sini gotururuk (yeni tr-ni)

        let input = $(this).next()                                     //inputu goturuk (productun sayi olan input)

        let count = parseInt($(input).val()) - 1;                      //ve inputun icindeki deyeri bidefe azaldiriq (yeni her minus a click etdikde sayi 1 defe azalsin)

        let nativePrice = parseFloat($(this).parent().prev().text())   //productun ilkin qiymetin gotururuk (yeni product price)

        let total = $(this).parent().next().children().eq(0);          //bir productun her row uzre (her tr-deki) umumi totalin goturuk

        let sum = 0;

        let grandTotal = $(".total-product").children().eq(0);          //butun productlari toplam giymetin deyismek ucun goturuk 

           
        if (count > 0) {                                                //ve eger count 1 den-yuxaridisa  deyisiklik ede bilerik, 1den az olduqda  inputun deyerini(productun sayini) 1den azaltmaq olmasin deye  wert qoyuruq
            $(input).val(count);                                       //elimizdeki countu inputun valuesine yazdiririq
            $.ajax({
                url: `card/DecreasetProductCount?id=${productId}`,      //productu countunu azaldan actiona request atiriq
                type: "Post",
                success: function (res) {
                    let productCount = res;                               //actiondan gelen azalmis productun sayi
                    let subtotal = nativePrice * productCount             //subtotal = productun ilkin giymeti ile indiki sayini vururuq
                    total.text(subtotal + ".00")                          //productun ara-totalina yazdiririq subtotali
                    for (const product of $(".table-product").children()) {     //tablede olan rowlari(productlari yebi tr-leri) fora saliriq
                        let total = parseFloat($(product).children().eq(6).text())   //butun productlarin ara-totallarini gotururuk
                        sum += total                                                //ve ara-totallari toplayiriq
                    }
                    $(grandTotal).text(sum + ".00");                            //topladiqimiz ara-totallari yazdiriq grandTotala

                }
            })
        }

    })




    //Increase Product from Basket (product sayini coxaltmaq basketde) 
    $(document).on("click", ".plus", function () {

        let productId = $(this).parent().parent().attr("data-id");              //hemin plusa aid olan productun Id-sini gotururuk

        let input = $(this).prev()                                                //inputu goturuk

        let count = parseInt($(input).val()) + 1;                                 //sonra inputun icindeki deyeri bidefe coxaldiriq

        $(input).val(count);                                                     //sonra coxaltdiqimiz deyeri  inputun valusuna elave edirik

        let nativePrice = parseFloat($(this).parent().prev().text())             //productun ilkin qiymetin gotururuk

        let total = $(this).parent().next().children().eq(0);                    //bir productun ara-totalin goturuk

        let sum = 0;

        let grandTotal = $(".total-product").children().eq(0);                  //butun productlarin toplam giymetin goturuk deyismek ucun


        $.ajax({
            url: `card/IncreaseProductCount?id=${productId}`,                     //productun countunu coxaldan actiona request atiriq
            type: "Post",
            success: function (res) {
                let countProduct = res;                                           //actiondan gelen coxalmis productun sayini (yeni res) variableye  beraber edirik
                let subtotal = nativePrice * countProduct                         //subtotal = productun ilkin giymetiynen indiki sayini vururuq
                total.text(subtotal + ".00")                                       //productun ara-totalina yazdiriq subtotali
                for (const product of $(".table-product").children()) {           //tablda olan rowlari(productlari) fora saliriq
                    let total = parseFloat($(product).children().eq(6).text())    //butun productlarin ara-totallarini gotururuk 
                    sum += total                                                  //ve ara-totallari toplayiriq 
                }
                $(grandTotal).text(sum + ".00");                                  //topladiqimiz ara-totallari yazdiriq grandTotala
            }
        })
        return false;
    })









    $.ajax({
        url: `card/index`,

        type: "Get",

        success: function (res) {



        }

    })
























    // HEADER

    $(document).on('click', '#search', function () {
        $(this).next().toggle();
    })

    $(document).on('click', '#mobile-navbar-close', function () {
        $(this).parent().removeClass("active");

    })
    $(document).on('click', '#mobile-navbar-show', function () {
        $('.mobile-navbar').addClass("active");

    })

    $(document).on('click', '.mobile-navbar ul li a', function () {
        if ($(this).children('i').hasClass('fa-caret-right')) {
            $(this).children('i').removeClass('fa-caret-right').addClass('fa-sort-down')
        }
        else {
            $(this).children('i').removeClass('fa-sort-down').addClass('fa-caret-right')
        }
        $(this).parent().next().slideToggle();
    })

    // SLIDER

    $(document).ready(function(){
        $(".slider").owlCarousel(
            {
                items: 1,
                loop: true,
                autoplay: true
            }
        );
      });

    // PRODUCT

    $(document).on('click', '.categories', function(e)
    {
        e.preventDefault();
        $(this).next().next().slideToggle();
    })

    $(document).on('click', '.category li a', function (e) {
        e.preventDefault();
        let category = $(this).attr('data-id');
        let products = $('.product-item');
        
        products.each(function () {
            if(category == $(this).attr('data-id'))
            {
                $(this).parent().fadeIn();
            }
            else
            {
                $(this).parent().hide();
            }
        })
        if(category == 'all')
        {
            products.parent().fadeIn();
        }
    })

    // ACCORDION 

    $(document).on('click', '.question', function()
    {   
       $(this).siblings('.question').children('i').removeClass('fa-minus').addClass('fa-plus');
       $(this).siblings('.answer').not($(this).next()).slideUp();
       $(this).children('i').toggleClass('fa-plus').toggleClass('fa-minus');
       $(this).next().slideToggle();
       $(this).siblings('.active').removeClass('active');
       $(this).toggleClass('active');
    })

    // TAB

    $(document).on('click', 'ul li', function()
    {   
        $(this).siblings('.active').removeClass('active');
        $(this).addClass('active');
        let dataId = $(this).attr('data-id');
        $(this).parent().next().children('p.active').removeClass('active');

        $(this).parent().next().children('p').each(function()
        {
            if(dataId == $(this).attr('data-id'))
            {
                $(this).addClass('active')
            }
        })
    })

    $(document).on('click', '.tab4 ul li', function()
    {   
        $(this).siblings('.active').removeClass('active');
        $(this).addClass('active');
        let dataId = $(this).attr('data-id');
        $(this).parent().parent().next().children().children('p.active').removeClass('active');

        $(this).parent().parent().next().children().children('p').each(function()
        {
            if(dataId == $(this).attr('data-id'))
            {
                $(this).addClass('active')
            }
        })
    })

    // INSTAGRAM

    $(document).ready(function(){
        $(".instagram").owlCarousel(
            {
                items: 4,
                loop: true,
                autoplay: true,
                responsive:{
                    0:{
                        items:1
                    },
                    576:{
                        items:2
                    },
                    768:{
                        items:3
                    },
                    992:{
                        items:4
                    }
                }
            }
        );
      });

      $(document).ready(function(){
        $(".say").owlCarousel(
            {
                items: 1,
                loop: true,
                autoplay: true
            }
        );
      });
})