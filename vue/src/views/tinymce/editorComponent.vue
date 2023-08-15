<template>
  <div class="container">
    <editor
      v-model="editorContent"
      api-key="2u5xxcts6ax2tqiexfsgphby9obxlq6kt2tsar8jn0knqlmz"
      :init="{
        
         height: 500,
         entity_encoding :'raw',
         menubar: true,
         plugins: [
           'advlist autolink lists link image charmap print preview anchor',
           'searchreplace visualblocks code fullscreen',
           'insertdatetime media table paste code help wordcount pageembed'
         ],
         toolbar:
           'undo redo | bold italic link bullist | insertUsername | formatselect | fontsizeselect | bold italic underline backcolor forecolor| \
           alignleft aligncenter alignright alignjustify | \
           bullist numlist outdent indent | removeformat | link image pageembed code | help',
          images_upload_handler: (blobInfo, success, failure, progress) => {
            handleUploadImg(blobInfo, success, failure, progress)
          },
          setup: function (editor) {
            var insertUsername = function () {
              editor.insertContent(`[Name]`);
            };

            var insertFirstName = function () {
              editor.insertContent(`[FirstName]`);
            };

            editor.ui.registry.addMenuButton('insertUsername', {
              icon: 'plus',
              fetch: function (callback) {
                var items = [
                  {
                    type: 'menuitem',
                    text: 'Insert name',
                    onAction: function () {
                      insertUsername();
                    }
                  },
                  {
                    type: 'menuitem',
                    text: 'Insert first name',
                    onAction: function () {
                      insertFirstName();
                    }
                  },
                ];
                callback(items);
              }
            });
          },
          images_upload_url: 'postAcceptor.ts',
          
          extended_valid_elements : 'iframe[src|frameborder|style|scrolling|class|width|height|name|align]'
       }"
      @onChange="emitValue"
    />
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Emit, Prop, Watch } from 'vue-property-decorator'
import iView from 'iview'
import Editor from '@tinymce/tinymce-vue'
import appconst from '../../lib/appconst'
@Component({
  components: {
    'editor': Editor
  }
})
export default class EditorComponent extends Vue {
  @Prop() value: any
  private editorContent: any = ''
  
  created() {
  }
  deactivated() {
    this.editorContent = ''
  }
  @Watch('value')
  setContent() {
    this.editorContent = this.value
  }

  @Emit('onChange') emitValue() {
    return this.editorContent
  }

  async handleUploadImg(blobInfo, success, failure, progress) {
    let formData = new FormData();
    formData.append('file', blobInfo.blob(), blobInfo.filename());
    let response = await this.$store.dispatch({
      type: 'email/uploadImage',
      data: formData
    })
    if(response.data.success){
      success(appconst.remoteServiceBaseUrl + response.data.result)
    }
    else failure('Invalid JSON: ' + response.data.error);
  }

}
</script>
<style scoped>
</style>

