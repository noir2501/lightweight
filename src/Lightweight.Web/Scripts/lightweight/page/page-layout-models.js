
ModuleModel = Backbone.Model.extend({
    defaults: {
        Id: null,
        Name: null
    },

    idAttribute: "Id"
});

ModuleCollection = Backbone.Collection.extend({
    model: ModuleModel,

    url: "GetModulesList"
});

WidgetModel = Backbone.Model.extend({
    defaults: {
        Id: null,
        Title: "",
        Width: 1,
        Height: 1
    },

    idAttribute: "Id",

    methodToUrl: function (method) {
        switch (method) {
            case "read":
                return "GetPageWidget?id=" + this.id;
            case "create":
                return "CreatePageWidget";
            case "update":
                return "UpdatePageWidget";
            case "delete":
                return "DeletePageWidget?id=" + this.id;
        }

        return null;
    },

    sync: function (method, model, options) {
        options = options || {};
        options.url = model.methodToUrl(method.toLowerCase());
        Backbone.sync(method, model, options);
    }
});

WidgetCollection = Backbone.Collection.extend({
    model: WidgetModel,

    url: function () {
        if (!this.pageId)
            throw "There is no page specified for this widget collection";

        return "GetPageWidgetsList?pageId=" + this.pageId;
    },

    initialize: function (options) {
        this.pageId = options.pageId;
    },

    comparator: function (model) {
        return model.get('Row') * 10 + model.get('Col');
    }
});