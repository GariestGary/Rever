void BlurPass_float(SamplerState ss, Texture2D tex, float2 uv, float mip, out float4 col) 
{
					/*if (_StandardDeviation == 0)
					{
						fixed4 c = tex2D(_MainTex, uv);
						c.rgb *= c.a;
						return c;
					}*/

					//init color variable
					col = 0;
					//uv = 0;

					float sum = 0;

					//iterate over blur samples
					for (float index = 0; index < _Samples; index++) 
					{
						//get the offset of the sample
						float offset = (index / (_Samples - 1) - 0.5) * _BlurSize;
						//get uv coordinate of sample
						float2 uvOffset = uv + float2(0, offset);

						uint3 coord = uint3(uvOffset.x, uvOffset.y, mip);

						//getting alpha
						float4 c = tex.Sample(ss, uvOffset);
						c.rgb *= c.a;

						
						//calculate the result of the gaussian function
						float stDevSquared = _StandardDeviation * _StandardDeviation;
						float gauss = (1 / sqrt(2 * 3.14159265359 * stDevSquared)) * pow(2.71828182846, -((offset * offset) / (2 * stDevSquared)));
						//add result to sum
						sum += gauss;
						//multiply color with influence from gaussian function and add it to sum color
						col += c * gauss;
					}
					//divide the sum of values by the amount of samples
					col = col / sum;

}