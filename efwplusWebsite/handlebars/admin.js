define(['jquery', 'common', "handlebars.min", "jquery.formfill", "text!../../handlebars/admin.html"], function ($, common, Handlebars, formfill, html_template) {

    function projecthandle() {
        //提交
        common.webapiPost('#projectForm', 'ProjectController', 'SaveProject', function (data) {
            alert("提交保存成功！");
            $('#projectID').val(data);
        });
        //查询
        $('#btn_searchproject').click(function () {
            common.webapiGet('ProjectController', 'GetProject', { projectID: $('#projectID').val() }, function (data) {
                data = common.toJson(data);
                if (data.length > 0) {
                    $("#projectForm").autofill(data[0]);
                } else {
                    $('#projectForm')[0].reset();
                }
            });
        });

        //清空
        $('#btn_addproject').click(function () {
            $('#projectForm')[0].reset();
        });
    }

    function articlehandle() {
        //提交
        common.webapiPost('#articleForm', 'articleController', 'Savearticle', function (data) {
            alert("提交保存成功！");
            $('#articleID').val(data);
        });
        //查询
        $('#btn_searcharticle').click(function () {
            common.webapiGet('articleController', 'Getarticle', { articleID: $('#articleID').val() }, function (data) {
                data = common.toJson(data);
                if (data.length > 0) {
                    $("#articleForm").autofill(data[0]);
                } else {
                    $('#articleForm')[0].reset();
                }
            });
        });

        //清空
        $('#btn_addarticle').click(function () {
            $('#articleForm')[0].reset();
        });
    }

    function classhandle() {
        //提交
        common.webapiPost('#classForm', 'DictionaryController', 'SaveClass', function (data) {
            alert("提交保存成功！");
            $('#classID').val(data);
        });
        //查询
        $('#btn_searchclass').click(function () {
            common.webapiGet('DictionaryController', 'GetClass', { classID: $('#classID').val() }, function (data) {
                data = common.toJson(data);
                if (data.length > 0) {
                    $("#classForm").autofill(data[0]);
                } else {
                    $('#classForm')[0].reset();
                }
            });
        });
        $('#btn_addclass').click(function () {
            $('#classForm')[0].reset();
        });
        $('#btn_classdata').click(function () {
            common.webapiGet('DictionaryController', 'GetClassData', {}, function (data) {
                alert(data);
            });
        });
    }

    function showpage(pageId, templates) {

        common.webapiGet('DictionaryController', 'ShowAdminData', {}, function (data) {
            common.loadtemplate(pageId, templates, html_template, { data: common.toJson(data) });
            //项目处理
            projecthandle();
            //文章处理
            articlehandle();
            //分类处理
            classhandle();
            //标签处理
        });
    }

    return {
        showpage: showpage
    };
});