/* LAYOUT view */

LayoutView = Backbone.View.extend({

    initialize: function (options) {
        $('#add-widget').on('click', $.proxy(this.addWidget, this));

        this.template = $('#layout-template').html();
        this.pageId = options.pageId;

        this.collection.on('reset', this.render, this);
        this.collection.on('add', this.widgetAdded, this);

        this.modulesView = options.modulesView;
        this.widgetViews = [];
    },

    render: function () {
        //console.log("rendering page layout...");
        var self = this;
        // enable gridster

        this.$gridster = this.$el.find(".gridster").gridster({
            widget_selector: ".g-widget",
            widget_margins: [10, 10],
            widget_base_dimensions: [360, 250],
            min_cols: 3,
            max_cols: 3,
            max_size_x: 3,
            helper: 'clone',
            avoid_overlapped_widgets: false,
            serialize_params: function ($w, wgd) {
                return { Id: $w.attr('data-id'), Col: wgd.col, Row: wgd.row, Width: wgd.size_x, Height: wgd.size_y };
            },
            draggable: {
                handle: '.fa-table',
                start: function (event, ui) {
                    console.log('dragging');
                },
                stop: function (event, ui) {
                    self.checkWidgetUpdates();
                }
            },
            resize: {
                enabled: true,
                max_size: [3, 3],
                start: function (event, ui, $widget) {
                    var widgetId = $widget.attr('data-id');

                    var widgetView = _.find(self.widgetViews, function (view) {
                        return view.model.id == widgetId;
                    });

                    widgetView.beginResize();
                },
                stop: function (event, ui, $widget) {
                    self.checkWidgetUpdates();

                    var widgetId = $widget.attr('data-id');

                    var widgetView = _.find(self.widgetViews, function (view) {
                        return view.model.id == widgetId;
                    });

                    widgetView.endResize();
                }
            }
        }).data('gridster');

        // render widgets by adding them to the grid
        //console.log("rendering widgets...");
        this.collection.sort().each(function (model) {
            this.widgetAdded(model);
        }, this);
    },

    addWidget: function () {
        //console.log("adding widget...");
        var self = this;
        var widget = new WidgetModel({ PageId: this.collection.pageId, ModuleId: this.modulesView.getSelectedId() });

        // save widget to db to get back its assigned Id
        widget.save(null, {
            success: function (model, response, options) {
                self.collection.add(widget);
            },
            // show error and destroy created model
            error: function (model, response, options) {
                widget.destroy();
            }
        });

        return widget;
    },

    widgetAdded: function (model) {
        // create the gridster widget
        var template = $('#layout-widget-container-template').html();
        var content = _.template(template, { Id: model.id });
        var widget = this.$gridster.add_widget(content, model.get('Width'), model.get('Height'), model.get('Col'), model.get('Row'));
        var module = this.modulesView.collection.findWhere({ Id: model.get('ModuleId') });

        // render the widget view
        var widgetView = new WidgetView({ el: widget, model: model, module: module, gridster: this.$gridster })
            .render()
            .on("removed", this.widgetRemoved, this);

        this.widgetViews.push(widgetView);

        // check if view moved (based on model)
        var position = _.findWhere(this.$gridster.serialize(), { Id: model.id });
        var moved = position.Col != model.get('Col') || position.Row != model.get('Row') || position.Height != model.get('Height') || position.Width != model.get('Width');

        // if moved, update its coords into db
        if (moved)
            this.updateWidget(model, position);
    },

    // get widget landing position and set it on model
    updateWidget: function (model, position) {
        console.log("updating widget...", model.get('Title'),
            'row:' + model.get('Row') + '->' + position.Row,
            'col:' + model.get('Col') + '->' + position.Col
        );


        model.set({ Col: position.Col, Row: position.Row, Width: position.Width, Height: position.Height });
        model.save(null, { type: 'POST' }); // update widget's assigned position into db
    },

    // remove widget view from views list and update remaining widget positions
    widgetRemoved: function (widgetView) {
        var l = this.widgetViews.length;
        this.widgetViews = _.filter(this.widgetViews, function (view) { return view.cid != widgetView.cid });
        this.checkWidgetUpdates();
    },

    // check all widgets models and positions and if needed call updateWidget
    checkWidgetUpdates: function () {
        var self = this;
        this.collection.each(function (model) {
            var position = _.findWhere(self.$gridster.serialize(), { Id: model.id });
            var moved = position.Col != model.get('Col') || position.Row != model.get('Row') || position.Height != model.get('Height') || position.Width != model.get('Width');
            if (moved)
                self.updateWidget(model, position);
        });
    }
});

/* WIDGET views */

WidgetView = Backbone.View.extend({

    events: {
        "click #remove-widget": "removeWidget",
        "click #save-widget": "saveWidget"
    },

    initialize: function (options) {
        this.template = $('#layout-widget-template').html();
        this.module = options.module;
        this.$gridster = options.gridster;
    },

    render: function () {
        //console.log("rendering widget...");
        var content = _.template(this.template, this.model.toJSON());
        this.$el.find('.g-widget').html(content);

        var self = this;

        // bind title editor
        var $title = this.$el.find('#widget-title');
        $title.editable({
            type: 'text',
            name: 'title',
            success: function (response, val) {
                self.model.save({ 'Title': val }, { type: 'POST' }); //update backbone model
            }
        });

        // render module editor view
        var $editorContainer = this.$el.find('.widget-main');
        this.$editorView = new EditorView({ el: $editorContainer, model: this.module, widgetView: this }).render(); // todo: model should be this.model and send module data separately

        return this;
    },

    saveWidget: function (e) {
        e.preventDefault();
        // get content from editor
        var content = this.$editorView.getContent();

        // send model update to the server
        this.model.save({ Content: content }, {
            type: 'POST'
        });
        
    },

    removeWidget: function (e) {
        e.preventDefault();
        if (!confirm('Are you sure you want to remove this widget?'))
            return;

        var self = this;
        this.model.destroy({
            wait: true,
            type: 'POST',
            success: function () {
                self.$gridster.remove_widget(self.$el, $.proxy(self.widgetRemoved, self));
            }
        });
    },

    widgetRemoved: function ($w) {
        this.trigger("removed", this);
        this.remove();
        //console.log("widget removed...");
    },

    beginResize: function () {
        this.$editorView.beginResize();
    },

    endResize: function () {
        this.$editorView.endResize();
    }

});

// todo: this should be an external view defined by the module
EditorView = Backbone.View.extend({
    initialize: function (options) {
        this.widgetView = options.widgetView; // the widget view
        var selector = '#module-editor-' + this.model.id + '-template';
        this.template = $(selector).html();
    },

    render: function () {
        this.$el.html(_.template(this.template, { Id: this.widgetView.model.cid }));

        this.$area = this.$el.find('textarea');

        var content = this.widgetView.model.get('Content');
        this.$area.val(content);

        var css = 'overflow: scroll';
        this.$area.css(css);

        // calculate editor height
        var widgetWidth = this.widgetView.model.get('Width');
        var widgetHeight = this.widgetView.model.get('Height');

        this.$area.markdown({
            autofocus: false,
            fullscreen: {
                enable: false
            },
            hiddenButtons: ['cmdPreview'],
            additionalButtons:
            [
                [{
                    name: "groupCustom",
                    data: [{
                        name: "cmdaPreview", toggle: false, title: "Preview", icon: "fa fa-search",
                        callback: function (e) { if (e.$isPreview) { e.hidePreview(); e.showEditor(); } else { e.showPreview(); e.enableButtons('cmdaPreview'); } }
                    }]
                }]
            ],
            height: this.getEditorHeight(widgetWidth, widgetHeight, 0),
            savable: false
        });

        return this;
    },

    getContent: function() {
        return this.$area.val();
    },

    beginResize: function () {
        this.$area.height(this.getEditorHeight(1, 1, 68));
    },

    endResize: function () {

        var widgetWidth = this.widgetView.model.get('Width');
        var widgetHeight = this.widgetView.model.get('Height');
        var editorHeight = this.getEditorHeight(widgetWidth, widgetHeight, 68);

        this.$area.height(editorHeight);
    },

    getEditorHeight: function (widgetWidth, widgetHeight, outerHeight) {
        var editorHeight = 230 * widgetHeight;
        if (widgetHeight > 1)
            editorHeight += 40 * (widgetHeight - 1);

        if (widgetWidth > 1) {
            editorHeight -= 26;
            outerHeight -= 26;
        }

        if (outerHeight < 0)
            outerHeight = 0;

        return editorHeight - outerHeight;
    }

});

/* MODULE views */

ModulesView = Backbone.View.extend({
    initialize: function (options) {
        this.collection.on('reset', this.render, this);
    },

    render: function () {
        //console.log("rendering modules...");
        var $container = this.$el;
        this.collection.each(function (model) {
            var view = new ModuleView({ model: model });
            $container.append(view.render().el);
        });
    },

    // returns the selected module Id
    getSelectedId: function () {
        return this.$el.val();
    }
});

ModuleView = Backbone.View.extend({
    initialize: function (options) {
        this.template = $('#module-template').html();
    },

    render: function () {
        var content = _.template(this.template, this.model.toJSON());
        this.el = content;

        return this;
    }
});