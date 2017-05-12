define(['jquery', 'common', "handlebars.min", "text!../../handlebars/about.html"], function ($, common, Handlebars, html_template) {

    function showpage(pageId, templates) {
            common.loadtemplate(pageId, templates, html_template, { data: [] });
    }

    return {
        showpage: showpage
    };
});