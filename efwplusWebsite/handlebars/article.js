define(['jquery', 'common', "handlebars.min", "text!../../handlebars/article.html"], function ($, common, Handlebars, html_template) {

    function showpage(pageId, templates, Id,searchKey) {
        common.webapiGet('ArticleController', 'ShowArticle', { classID: Id, searchKey: searchKey }, function (data) {
            common.loadtemplate(pageId, templates, html_template, { data: common.toJson(data) });

            $('#btn_search').click(function () {
                var searchKey= $('#txtSearch').val();
                if (searchKey.length > 0)
                    showpage(pageId, templates, '', searchKey);
            });
        });
    }

    return {
        showpage: showpage
    };
});