define(["handlebars.min", "common"], function (Handlebars, common) {
    
    var templates;//模板内容
    templates = new Array();//handlebars模板对象

    //初始化
    function init() {
        loadrouter();
        loadhome();
    }

   
    //加载路由
    function loadrouter() {
        var opencontent = function (pageId,Id) {

            $('#collapse-head').find('li').removeClass("am-active");
            $('#' + pageId).addClass("am-active");
            showpage(pageId,Id);
        };

        var routes = {
            '/:pageId/:Id': [opencontent],
            '/:pageId': [opencontent],
        };
        var router = Router(routes);
        router.init();
    }

    var isDefaultHome = true;
    //加载首页
    function loadhome() {
        if (isDefaultHome == true)
            showpage('home');
    }

    //动态加载页面
    function showpage(pageId, Id) {
        isDefaultHome = false;
        $('#content_body').html("");//先清空

        require(["../../handlebars/" + pageId], function (page) {
            page.showpage(pageId, templates, Id);
        });
    }

    return {
        init: init
    };
});