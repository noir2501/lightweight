

var PageLayoutRouter = Backbone.Router.extend({
    routes: {
        "": "index",
    },

    index: function () {
        this.loaded = this.loaded || this.load(this.pageId);
    },

    load: function (pageId) {

        if (!this.modules)
            this.modules = new ModuleCollection();

        // set modules functions
        

        if (!this.widgets)
            this.widgets = new WidgetCollection({ pageId: pageId });

        this.modulesView = new ModulesView({ el: '#modules-list', collection: this.modules });
        this.layoutView = new LayoutView({ el: '#page-layout', collection: this.widgets, modulesView: this.modulesView });

        var self = this;
        this.modules.fetch({
            reset: true, success: function () {
                self.widgets.fetch({ reset: true });
            }
        });

    }
});