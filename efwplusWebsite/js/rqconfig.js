require.config({
    baseUrl: 'js/lib',
    map: {
        '*': {
            'css': 'css.min',
            'text': 'text'
        }
    },
    paths: {
        //text: 'text',
        //css: 'css.min',
        "jquery": 'jquery.min',
        "amazeui": '../../uiframe/amazeui/js/amazeui.min',
        "jquery.json": 'jquery.json-2.3.min',
        "jquery.formfill": 'jquery.formautofill.min',
        "common": '../common',
        //"login": "../login",
        //"app":"app",
        "index": '../index'
    },
    shim: {
        "amazeui": ["jquery"],
        //"amazeui.tree.min": ["jquery", "css!../css/amazeui.tree.min.css"],
        "jquery.cookie": ["jquery"],
        //"jquery.formatDateTime.min": ["jquery"],
        "jquery.json": ["jquery"],
        "jquery.formfill": ["jquery"],
        //"app": ["jquery", "amazeui"],
        //"login": ["amazeui"],
        "index": ["amazeui","jquery", "jquery.cookie", "director.min"]
    },
    waitSeconds: 50
});

//requirejs(["amazeui.min","app", "index"]);