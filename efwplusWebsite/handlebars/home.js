define(['jquery', 'common', "handlebars.min", "text!../../handlebars/home.html"], function ($, common, Handlebars, html_template) {

    function showpage(pageId, templates) {
        common.webapiGet('ProjectController', 'ShowHomeProject', {}, function (data) {
            common.loadtemplate(pageId, templates, html_template, { data: common.toJson(data) });
        });
    }

    return {
        showpage: showpage
    };
});