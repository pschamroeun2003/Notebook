<template>
    <div class="flex items-center justify-center min-h-screen bg-gray-100">
        <div class="w-full max-w-sm p-6 bg-white rounded shadow-lg">
            <h2 class="mb-6 text-2xl font-bold text-center text-gray-800">Register</h2>
            <form @submit.prevent="handleRegister">
                <div class="mb-4">
                    <label for="name" class="block mb-2 text-sm font-medium text-gray-700">Full Name</label>
                    <input v-model="form.name" type="text" id="name" placeholder="Enter your full name"
                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring focus:ring-blue-200 focus:outline-none"
                        required />
                </div>
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
                <div class="mb-4">
                    <label for="confirmPassword" class="block mb-2 text-sm font-medium text-gray-700">Confirm
                        Password</label>
                    <input v-model="form.confirmPassword" type="password" id="confirmPassword"
                        placeholder="Confirm your password"
                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring focus:ring-blue-200 focus:outline-none"
                        required />
                </div>
                <button type="submit"
                    class="w-full px-4 py-2 text-white bg-blue-600 rounded-lg hover:bg-blue-700 focus:outline-none focus:ring focus:ring-blue-200">
                    Register
                </button>
            </form>
            <p class="mt-4 text-sm text-center text-gray-600">
                Do you have an account?
                <router-link to="/" class="text-blue-600 hover:underline">Login here</router-link>.
            </p>
        </div>
    </div>
</template>

<script lang="ts">
import axios from '@/axios';
import Swal from 'sweetalert2';
import { defineComponent, reactive } from 'vue';
import { useRouter } from 'vue-router';

export default defineComponent({
    name: 'RegisterForm',
    setup() {
        const form = reactive({
            name: '',
            email: '',
            password: '',
            confirmPassword: '',
        });

        const router = useRouter();

        const handleRegister = async () => {
            if (form.password !== form.confirmPassword) {
                Swal.fire({
                    title: 'Error!',
                    text: 'Passwords do not match!',
                    icon: 'error',
                    confirmButtonText: 'OK',
                });
                return;
            }

            try {
                const response = await axios.post('api/auth/register', {
                    Username: form.name,
                    Password: form.password,
                    Email: form.email,
                });

                // Check if registration was successful
                if (response.data.status) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'Registration successful!',
                        icon: 'success',
                        confirmButtonText: 'OK',
                    });

                    // Extract token, refreshToken, and userId from the response
                    const { token, refreshToken, user } = response.data;
                    const userId = user.id;  // Correct field is 'id' instead of 'userId'

                    // Store the token, refreshToken, and userId in localStorage
                    localStorage.setItem('authToken', token);
                    localStorage.setItem('refreshToken', refreshToken);
                    localStorage.setItem("userId", userId.toString());  // Store the userId in localStorage

                    setTimeout(() => {
                        router.push('/notebook'); // Replace with the correct route for the notebook
                    }, 1000);
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: response.data.message || 'There was an issue registering. Please try again.',
                        icon: 'error',
                        confirmButtonText: 'OK',
                    });
                }

            } catch (error) {
                console.error('There was an error registering:', error);
                Swal.fire({
                    title: 'Error!',
                    text: 'There was an issue registering. Please try again later.',
                    icon: 'error',
                    confirmButtonText: 'OK',
                });
            }
        };


        return {
            form,
            handleRegister,
        };
    },
});
</script>

<style scoped>
/* Add any additional custom styles if needed */
</style>
