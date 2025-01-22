<template>
    <div class="flex items-center justify-center min-h-screen bg-gray-100">
        <div class="w-full max-w-sm p-6 bg-white rounded shadow-lg">
            <h2 class="mb-6 text-2xl font-bold text-center text-gray-800">Login</h2>
            <p v-if="errorMessage" class="mt-4 text-sm text-red-500 text-center">{{ errorMessage }}</p>

            <form @submit.prevent="handleSubmit">
                <div class="mb-4">
                    <label for="email" class="block mb-2 text-sm font-medium text-gray-700">Email</label>
                    <input v-model="form.email" type="email" id="email" placeholder="Enter your email"
                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring focus:ring-blue-200 focus:outline-none"
                        required />
                </div>
                <div class="mb-4">
                    <label for="password" class="block mb-2 text-sm font-medium text-gray-700">Password</label>
                    <input v-model="form.password" type="password" id="password" placeholder="Enter your password"
                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring focus:ring-blue-200 focus:outline-none"
                        required />
                </div>
                <button type="submit"
                    class="w-full px-4 py-2 text-white bg-blue-600 rounded-lg hover:bg-blue-700 focus:outline-none focus:ring focus:ring-blue-200">
                    Login
                </button>
            </form>
            <p class="mt-4 text-sm text-center text-gray-600">
                Don't have an account?
                <router-link to="/registerform" class="text-blue-600 hover:underline">Register here</router-link>.
            </p>
        </div>
    </div>
</template>

<script lang="ts">
import axios from '@/axios';
import Swal from 'sweetalert2';
import { defineComponent, reactive, ref } from "vue";
import { useRouter } from 'vue-router';

export default defineComponent({
    name: "LoginForm",
    setup() {
        const form = reactive({
            email: "",
            password: "",
        });
        const router = useRouter();

        const errorMessage = ref<string | null>(null);

        const handleSubmit = async () => {
            try {
                const response = await axios.post("api/auth/login", {
                    Email: form.email,
                    Password: form.password,
                });

                if (response.data.status) {
                    const { token, refreshToken, id } = response.data.user;  
                    localStorage.setItem("jwtToken", token);
                    localStorage.setItem("refreshToken", refreshToken);
                    localStorage.setItem("userId", id.toString());  

                    Swal.fire({
                        icon: 'success',
                        title: 'Login successful!',
                        text: 'Redirecting to your notebook...',
                        timer: 1000, 
                        showConfirmButton: false,
                    });

                    setTimeout(() => {
                        router.push('/notebook');
                    }, 1000);
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Login Failed',
                        text: response.data.message || 'Invalid credentials.',
                    });
                }
            } catch (error: any) {
                console.error("Login error:", error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: error.response?.data?.message || 'An unexpected error occurred.',
                });
            }
        };

        return {
            form,
            handleSubmit,
            errorMessage,
        };
    },
});
</script>
