<template>
  <DashboardTitle title="Link details" />
  <div
    class="shadow-md p-4 rounded-xl max-w-screen-lg w-full"
    :class="{ 'shadow-yellow-500': link.isCustom }"
  >
    <Badge :class="badgeClass">
      <span class="flex items-center">
        <Icon :name="statusIconBadge" class="w-4 h-4 mr-2" />
        {{ link.active ? 'Active' : 'Inactive' }}
      </span>
    </Badge>
    <div class="flex w-full space-x-2">
      <div class="w-3/4 space-y-6">
        <Input
          :class="[
            'disabled:border-none',
            'disabled:font-semibold',
            'disabled:text-xl',
            'disabled:bg-transparent',
            'focus-visible:outline-none',
            'focus-visible:ring-0',
            'focus-visible:ring-transparent',
            'focus-visible:ring-offset-0',
            'border-none text-yellow-500',
            'text-xl bg-transparent',
            link.title ? 'text-violet-500' : 'text-pink-500',
            link.isCustom ? 'text-yellow-500' : ''
          ]"
          type="text"
          v-model="titleValue"
          placeholder="Type the title of the link"
          :disabled="!isEditing"
        />
        <LinkDisplay title="Link" :url="link.url" />
        <LinkDisplay title="Orginal" :url="link.orginalUrl" />
        <div class="flex w-full space-x-4">
          <div class="w-1/2 bg-slate-200 p-2 rounded-xl">
            <p>Created</p>
            <p class="md:text-md text-sm text-violet-500">
              {{ dayjs(link.created).format('DD.MM.YYYY HH:mm') }}
            </p>
          </div>
          <div class="w-1/2 bg-slate-200 p-2 rounded-xl">
            <p>Updated</p>
            <p class="md:text-md text-sm text-violet-500">
              {{
                link.updated
                  ? dayjs(link.updated).format('DD.MM.YYYY HH:mm')
                  : 'Not updated'
              }}
            </p>
          </div>
        </div>
      </div>
      <div class="flex flex-col w-1/4 justify-between">
        <Button
          type="button"
          class="bg-violet-500 hover:bg-violet-700 transition-colors duration-300"
          @click="handleEditInputDisabled"
          :disabled="linkStore.isLoading || isRequesting"
        >
          <Icon name="lucide:pencil" class="w-4 h-4" />
        </Button>
        <Button
          type="button"
          class="bg-cyan-500 hover:bg-cyan-700 transition-colors duration-300"
          @click="handleActieInactive"
          :disabled="isEditing || linkStore.isLoading || isRequesting"
        >
          <Icon :name="statusIconButton" class="w-4 h-4" />
        </Button>
        <Button
          type="button"
          class="bg-blue-500 hover:bg-blue-700 transition-colors duration-300"
          @click="
            handleUpdate({
              title: titleValue,
              active: linkStore.link?.active
            })
          "
          :disabled="isEditing || linkStore.isLoading || isRequesting"
        >
          <Icon name="lucide:save" class="w-4 h-4" />
        </Button>
        <Button
          type="button"
          class="bg-red-500 hover:bg-red-700 transition-colors duration-300"
          @click="handleDelete"
          :disabled="isEditing || linkStore.isLoading || isRequesting"
        >
          <Icon name="lucide:trash-2" class="w-4 h-4" />
        </Button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { useToast } from '~/components/ui/toast'

const props = defineProps({
  //   linkId: {
  //     type: String,
  //     required: true
  //   },
  //   url: {
  //     type: String,
  //     required: true
  //   },
  //   orginalUrl: {
  //     type: String,
  //     required: true
  //   },
  //   title: {
  //     type: String,
  //     required: true
  //   },
  //   isCustom: {
  //     type: Boolean,
  //     required: true
  //   },
  //   active: {
  //     type: Boolean,
  //     required: true
  //   },
  //   created: {
  //     type: Date,
  //     required: true
  //   },
  //   updated: {
  //     type: Date,
  //     required: false
  //   }
  link: {
    type: Object as PropType<LinkResponse>,
    required: true
  },
  title: {
    type: String,
    required: true
  }
})

const badgeClass = computed(() => [
  'mb-2',
  linkStore.link.active ? 'bg-green-500' : 'bg-red-500'
])

const handleActieInactive = () => {
  linkStore.link.active = !linkStore.link.active
}

const statusIconBadge = computed(() =>
  linkStore.link.active ? 'lucide:shield-check' : 'lucide:shield-x'
)

const statusIconButton = computed(() =>
  !linkStore.link.active ? 'lucide:shield-check' : 'lucide:shield-x'
)

const isEditing = ref(false)
const isRequesting = ref(false)
const { $api } = useNuxtApp()
const useRepository = repository($api)
const { toast } = useToast()
const titleValue = ref('')
const linkStore = useLinkStore()
const dayjs = useDayjs()

const handleEditInputDisabled = () => {
  isEditing.value = !isEditing.value
}
const renderTitleValue = ref(props.link.title)
const renderActiveValue = ref(props.link.active)

const handleDelete = async () => {
  const data = await useRepository.deleteLink(props.link.id)

  if (data.success) {
    linkStore.$reset()
    linkStore.isLoading = false
    navigateTo('/dashboard')
  }
}

const handleUpdate = async (updateLink: EditLinkForm) => {
  if (
    updateLink.title === renderTitleValue.value &&
    updateLink.active === renderActiveValue.value
  ) {
    return
  }
  isRequesting.value = true
  const data = await useRepository
    .updateLink(props.link.id, {
      active: updateLink.active,
      title: updateLink.title
    })
    .finally(() => {
      isRequesting.value = false
    })

  if (data.success) {
    linkStore.link = {
      ...linkStore.link,
      ...(updateLink.title !== linkStore.link?.title && {
        title: updateLink.title
      }),
      ...(updateLink.active !== linkStore.link?.active && {
        active: updateLink.active
      })
    }

    renderActiveValue.value = linkStore.link?.active
    renderTitleValue.value = linkStore.link?.title

    toast({
      description: 'Link updated successfully',
      variant: 'success'
    })
  }
}

onMounted(() => {
  titleValue.value = props.link.title
})
</script>
