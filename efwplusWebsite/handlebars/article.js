define(['jquery', 'common', "handlebars.min", "text!../../handlebars/article.html"], function ($, common, Handlebars, html_template) {

    function showpage(pageId, templates, Id) {
        common.webapiGet('ArticleController', 'ShowArticle', { classID: Id }, function (data) {
            common.loadtemplate(pageId, templates, html_template, { data: common.toJson(data) });
        });
    }

    return {
        showpage: showpage
    };
});