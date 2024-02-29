<template>
  <DashboardWrapper>
    <DashboardTitle title="Settings" />
    <Separator class="my-4" />
    <div class="flex flex-col space-y-4 bg-slate-200 p-6 rounded-xl">
      <div class="flex items-center justify-between">
        <h2 class="text-2xl font-semibold">Account</h2>
      </div>
      <div class="mt-4">
        <p class="text-gray-500 text-xl">Email</p>
        <p class="text-lg font-semibold">{{ authStore.user?.email }}</p>
      </div>
      <div class="mt-4">
        <p class="text-gray-500 text-xl">Role</p>
        <div class="flex space-x-4 mt-2">
          <Badge v-if="!authStore.user?.isAdmin" class="bg-gray-500">
            User
          </Badge>
          <Badge v-if="authStore.user?.isAdmin" class="bg-violet-500">
            Admin
          </Badge>
          <Badge v-if="authStore.user?.isSubscribed" class="bg-yellow-500">
            Premium
          </Badge>
        </div>
      </div>
      <div class="mt-4">
        <p class="text-gray-500 text-xl">Subscription</p>
        <p class="text-lg font-semibold">
          {{ authStore.user?.isSubscribed ? 'Active' : 'Inactive' }}
        </p>
      </div>
      <div class="mt-4">
        <p class="text-gray-500 text-xl">Billing</p>
        <div class="flex space-x-4 mt-2">
          <Button
            v-if="authStore.user?.isSubscribed"
            variant="secondary"
            @click="handleCreateBillingPortal"
          >
            <span v-if="isLoading">
              <Spinner />
            </span>
            <span v-else>Manage billing</span>
          </Button>
          <Button v-else @click="handleCheckout">
            <span v-if="isLoading">
              <Spinner />
            </span>
            <span v-else>Subscribe</span>
          </Button>
        </div>
        <div class="mt-4">
          <p class="text-gray-500 text-xl">Delete account</p>
          <p class="text-sm text-red-500 mt-2">
            This action is irreversible. All your data will be lost.
          </p>
          <Button
            variant="destructive"
            class="mt-2"
            v-if="!isSuccess"
            @click="handleDeleteAccountSendCode"
          >
            <span v-if="isLoadingDelete">
              <Spinner />
            </span>
            <span v-else>Delete</span>
          </Button>
          <form class="w-2/3" @submit="onSubmitDelete" v-else>
            <FormField v-slot="{ componentField }" name="pin">
              <FormItem>
                <FormControl>
                  <PinInput
                    id="pin-input"
                    placeholder="○"
                    class="flex gap-2 items-center mt-1"
                    otp
                    type="number"
                    :name="componentField.name"
                    @update:model-value="
                      (arrStr: string[]) => {
                        setValues({
                          pin: arrStr.filter(Boolean)
                        })
                      }
                    "
                  >
                    <PinInputInput
                      v-for="(id, index) in 6"
                      :key="id"
                      :index="index"
                    />
                  </PinInput>
                </FormControl>
                <p class="text-red-500 h-6">{{ errors.pin }}</p>
              </FormItem>
            </FormField>
            <p class="text-violet-500 text-md mt-4">
              {{
                isActiveSubscriptions
                  ? 'Your subscription is active. It will be cancelled. Are you sure?'
                  : ''
              }}
            </p>
            <Button class="mt-2" variant="destructive">
              <span v-if="isLoadingDelete">
                <Spinner />
              </span>
              <span v-else>DELETE</span></Button
            >
          </form>
        </div>
      </div>
    </div>
  </DashboardWrapper>
</template>

<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import * as z from 'zod'
import { PinInput, PinInputInput } from '@/components/ui/pin-input'
import { FormControl, FormField, FormItem } from '@/components/ui/form'
definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

useHead({
  title: 'TinyPath - Settings',
  meta: [
    {
      name: 'description',
      content: 'Dashboard page'
    }
  ]
})

const formSchema = toTypedSchema(
  z.object({
    pin: z.array(z.coerce.string()).length(6, { message: 'Invalid input' })
  })
)

const { handleSubmit, setValues, errors } = useForm({
  validationSchema: formSchema,
  initialValues: {
    pin: []
  }
})

const { $api } = useNuxtApp()
const useRepository = repository($api)
const isLoading = ref(false)
const isSuccess = ref(false)
const isActiveSubscriptions = ref(false)
const isLoadingDelete = ref(false)

const handleDeleteAccountSendCode = async () => {
  isLoadingDelete.value = true
  const data = await useRepository.sendEmailWithDeletedCode().finally(() => {
    isLoadingDelete.value = false
  })

  if (data) {
    isActiveSubscriptions.value = data.isActiveSubscriptions
    isSuccess.value = data.success
  }
}

const onSubmitDelete = handleSubmit(async (value) => {
  isLoadingDelete.value = true
  const joinPin = value.pin.join('')
  const data = await useRepository.deleteAccount(joinPin).finally(() => {
    isLoadingDelete.value = false
  })

  if (data.success) {
    authStore.logout()
    navigateTo('/')
  }
})

const handleCreateBillingPortal = async () => {
  isLoading.value = true
  const data = await useRepository.createBillingPortal().finally(() => {
    isLoading.value = false
  })

  if (data.link) {
    window.location.href = data.link
  }
}

const handleCheckout = async () => {
  isLoading.value = true
  const checkoutPrice = {
    priceCode: 'price_1Ocx5WAx2Zy2VacOjBvFRwak'
  }

  const data = await useRepository
    .createCheckoutSession(checkoutPrice)
    .finally(() => {
      isLoading.value = false
    })

  if (data.link) {
    window.location.href = data.link
  }
}

const authStore = useAuthStore()
</script>
